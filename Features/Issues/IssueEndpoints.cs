using backend.Infrastructure;
using backend.Infrastructure.Services;
using backend.Models;
using backend.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace backend.Features.Issues
{
    public static class IssueEndpoints
    {
        public static void MapIssueEndpoints(this IEndpointRouteBuilder app)
        {
            var adminGroup = app.MapGroup("/admin").RequireAuthorization("AdminOnly");
            
            adminGroup.MapGet("/issues", async (AppDbContext db, IConnectionMultiplexer redis, IStorageService storage, HttpContext ctx, IssueStatus? status, int page = 1, int pageSize = 10) =>
            {
                var cache = redis.GetDatabase();
                var cacheKey = $"admin:issues:{status}:{page}:{pageSize}";

                var cached = await cache.StringGetAsync(cacheKey);
                if (!cached.IsNullOrEmpty)
                    return Results.Ok(JsonSerializer.Deserialize<object>(cached!));

                var query = db.Issues
                    .Where(i => !i.IsDeleted)
                    .Include(i => i.User)
                    .Include(i => i.AssignedTo)
                    .AsQueryable();

                if (status.HasValue)
                    query = query.Where(i => i.Status == status.Value);

                var data = await query
                    .OrderByDescending(i => i.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = data.Select(i => new IssueResponseDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    Status = i.Status ?? IssueStatus.Open,
                    Block = i.Block,
                    RoomNumber = i.RoomNumber,
                    CreatedAt = i.CreatedAt,
                    AssignedStaffName = i.AssignedTo != null ? i.AssignedTo.Name : "Unassigned",
                    ImageUrl = i.ImagePath != null ? storage.GetUrl(i.ImagePath, ctx) : null
                });

                await cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(result), TimeSpan.FromMinutes(5));
                return Results.Ok(result);
            });

            adminGroup.MapPut("/issues/{id}/assign/{staffId}", async (AppDbContext db, IConnectionMultiplexer redis, long id, long staffId) =>
            {
                var issue = await db.Issues.FindAsync(id);
                if (issue == null || issue.IsDeleted) return Results.NotFound();

                var staff = await db.Users.FindAsync(staffId);
                if (staff == null || staff.Role != UserRole.Staff)
                    return Results.BadRequest("Invalid staff user");

                issue.AssignedToId = staffId;
                issue.Status = IssueStatus.InProgress;
                await db.SaveChangesAsync();

                await InvalidateAdminCache(redis);
                return Results.Ok(new { message = "Issue assigned to staff", issue.Id, issue.Status });
            });

            adminGroup.MapDelete("/issues/{id}", async (AppDbContext db, IConnectionMultiplexer redis, IStorageService storage, long id) =>
            {
                var issue = await db.Issues.FindAsync(id);
                if (issue == null || issue.IsDeleted) return Results.NotFound();

                issue.IsDeleted = true;
                if (!string.IsNullOrEmpty(issue.ImagePath))
                {
                    await storage.DeleteFileAsync(issue.ImagePath);
                    issue.ImagePath = null;
                }
                
                await db.SaveChangesAsync();

                await InvalidateAdminCache(redis);
                return Results.Ok(new { message = "Issue deleted successfully" });
            });

            app.MapPost("/issues/report", async (HttpContext ctx, AppDbContext db, IConnectionMultiplexer redis, IStorageService storage, IValidator<CreateIssueDto> validator) =>
            {
                var form = await ctx.Request.ReadFormAsync();
                
                var dto = new CreateIssueDto
                {
                    Title = form["Title"].ToString(),
                    Description = form["Description"].ToString(),
                    Block = form["Block"].ToString(),
                    RoomNumber = form["RoomNumber"].ToString()
                };

                var validation = await validator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return Results.BadRequest(validation.Errors.Select(e => e.ErrorMessage));

                var issue = new Issue
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Block = dto.Block,
                    RoomNumber = dto.RoomNumber,
                    UserId = SecurityHelper.GetUserId(ctx),
                    Status = IssueStatus.Open,
                    CreatedAt = DateTime.UtcNow
                };

                db.Issues.Add(issue);
                await db.SaveChangesAsync(); // Save first to get ID

                var image = form.Files.GetFile("Image");
                if (image != null)
                {
                    // Validation: MIME Type & Size (5MB)
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                    if (!allowedTypes.Contains(image.ContentType))
                        return Results.BadRequest("Only JPEG/PNG images are allowed.");

                    if (image.Length > 5 * 1024 * 1024)
                        return Results.BadRequest("File size exceeds 5MB limit.");

                    // Filename: {id}.jpeg
                    var fileName = $"{issue.Id}.jpeg";
                    issue.ImagePath = await storage.SaveFileAsync(image, fileName);
                    await db.SaveChangesAsync();
                }

                await InvalidateAdminCache(redis);
                return Results.Ok(new { message = "Issue created", issue.Id, imageUrl = issue.ImagePath != null ? storage.GetUrl(issue.ImagePath, ctx) : null });
            })
            .RequireAuthorization("StudentOnly")
            .RequireRateLimiting("issue");

            var staffGroup = app.MapGroup("/staff").RequireAuthorization("StaffOrAdmin");

            staffGroup.MapGet("/issues", async (HttpContext ctx, AppDbContext db, IStorageService storage) =>
            {
                var userId = SecurityHelper.GetUserId(ctx);
                var issues = await db.Issues
                    .Where(i => i.AssignedToId == userId && !i.IsDeleted)
                    .Include(i => i.User)
                    .OrderByDescending(i => i.CreatedAt)
                    .Select(i => new IssueResponseDto
                    {
                        Id = i.Id,
                        Title = i.Title,
                        Description = i.Description,
                        Status = i.Status ?? IssueStatus.Open,
                        Block = i.Block,
                        RoomNumber = i.RoomNumber,
                        CreatedAt = i.CreatedAt,
                        AssignedStaffName = i.AssignedTo != null ? i.AssignedTo.Name : "Me",
                        ImageUrl = i.ImagePath != null ? storage.GetUrl(i.ImagePath, ctx) : null
                    })
                    .ToListAsync();

                return Results.Ok(issues);
            });

            staffGroup.MapPut("/issues/{id}/status", async (HttpContext ctx, AppDbContext db, IConnectionMultiplexer redis, IValidator<UpdateStatusDto> validator, long id, UpdateStatusDto dto) =>
            {
                var validation = await validator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return Results.BadRequest(validation.Errors.Select(e => e.ErrorMessage));

                var userId = SecurityHelper.GetUserId(ctx);
                var issue = await db.Issues.FindAsync(id);

                if (issue == null || issue.IsDeleted) return Results.NotFound();
                if (issue.AssignedToId != userId) return Results.Forbid();

                if (issue.Status == IssueStatus.Resolved && dto.Status == IssueStatus.Open)
                    return Results.BadRequest("Cannot reopen resolved issue");

                issue.Status = dto.Status;
                await db.SaveChangesAsync();

                await InvalidateAdminCache(redis);
                return Results.Ok(new { message = "Status updated" });
            });

            app.MapGet("/student/issues", async (HttpContext ctx, AppDbContext db, IStorageService storage) =>
            {
                var userId = SecurityHelper.GetUserId(ctx);
                var issues = await db.Issues
                    .Where(i => i.UserId == userId && !i.IsDeleted)
                    .OrderByDescending(i => i.CreatedAt)
                    .Select(i => new IssueResponseDto
                    {
                        Id = i.Id,
                        Title = i.Title,
                        Description = i.Description,
                        Status = i.Status ?? IssueStatus.Open,
                        Block = i.Block,
                        RoomNumber = i.RoomNumber,
                        CreatedAt = i.CreatedAt,
                        AssignedStaffName = i.AssignedTo != null ? i.AssignedTo.Name : "Unassigned",
                        ImageUrl = i.ImagePath != null ? storage.GetUrl(i.ImagePath, ctx) : null
                    })
                    .ToListAsync();

                return Results.Ok(issues);
            })
            .RequireAuthorization("StudentOnly");
        }

        private static async Task InvalidateAdminCache(IConnectionMultiplexer redis)
        {
            var endpoints = redis.GetEndPoints();
            var server = redis.GetServer(endpoints.First());
            var keys = server.Keys(pattern: "admin:issues:*");
            var db = redis.GetDatabase();
            foreach (var key in keys)
            {
                await db.KeyDeleteAsync(key);
            }
        }
    }
}
