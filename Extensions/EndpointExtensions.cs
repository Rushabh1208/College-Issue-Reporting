using backend.Models;
using backend.Enums;
using backend.DTOs;
using backend.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace backend.Extensions
{
    public static class EndpointExtensions
    {
        public static void MapAppEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapAuthEndpoints();
            app.MapAdminEndpoints();
            app.MapIssueEndpoints();
            app.MapStaffEndpoints();
            app.MapStudentEndpoints();
            app.MapGet("/", () => "Campus API is running...");
        }

        private static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/login", async (AppDbContext db, LoginUserDto loginUser, IConfiguration config) =>
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email);

                if (user == null || user.PasswordHash != SecurityHelper.HashPassword(loginUser.Password))
                    return Results.Unauthorized();

                return Results.Ok(new { token = SecurityHelper.GenerateToken(user, config) });
            }).RequireRateLimiting("login");

            app.MapPost("/register", async (AppDbContext db, RegisterDto dto) =>
            {
                if (await db.Users.AnyAsync(u => u.Email == dto.Email))
                    return Results.BadRequest("Email already registered");

                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    PasswordHash = SecurityHelper.HashPassword(dto.Password),
                    Role = UserRole.Student 
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();

                return Results.Ok(new { message = "User registered successfully", user.Id, user.Email });
            });
        }

        private static void MapAdminEndpoints(this IEndpointRouteBuilder app)
        {
            var adminGroup = app.MapGroup("/admin").RequireAuthorization("AdminOnly");

            adminGroup.MapGet("/users", async (AppDbContext db) => 
                await db.Users.ToListAsync());

            adminGroup.MapGet("/issues", async (AppDbContext db, IConnectionMultiplexer redis, IssueStatus? status, int page = 1, int pageSize = 10) =>
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
                    Status = i.Status.ToString()!,
                    Block = i.Block,
                    RoomNumber = i.RoomNumber,
                    CreatedAt = i.CreatedAt,
                    AssignedStaffName = i.AssignedTo != null ? i.AssignedTo.Name : "Unassigned"
                });

                await cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(result), TimeSpan.FromSeconds(60));
                return Results.Ok(result);
            });

            adminGroup.MapPut("/issues/{id}/assign/{staffId}", async (AppDbContext db, long id, long staffId) =>
            {
                var issue = await db.Issues.FindAsync(id);
                if (issue == null || issue.IsDeleted) return Results.NotFound();

                var staff = await db.Users.FindAsync(staffId);
                if (staff == null || staff.Role != UserRole.Staff)
                    return Results.BadRequest("Invalid staff user");

                issue.AssignedToId = staffId;
                issue.Status = IssueStatus.InProgress;
                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Issue assigned to staff", issue.Id, issue.Status });
            });

            adminGroup.MapDelete("/issues/{id}", async (AppDbContext db, long id) =>
            {
                var issue = await db.Issues.FindAsync(id);
                if (issue == null || issue.IsDeleted) return Results.NotFound();

                issue.IsDeleted = true;
                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Issue deleted successfully" });
            });
        }

        private static void MapIssueEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/issues/report", async (HttpContext ctx, AppDbContext db, IValidator<CreateIssueDto> validator, CreateIssueDto dto) =>
            {
                var validation = await validator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return Results.BadRequest(validation.Errors.Select(e => e.ErrorMessage));

                var issue = new Issue
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Block = dto.Block,
                    RoomNumber = dto.RoomNumber,
                    UserId = SecurityHelper.GetUserId(ctx, db),
                    Status = IssueStatus.Open,
                    CreatedAt = DateTime.UtcNow
                };

                db.Issues.Add(issue);
                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Issue created", issue.Id });
            })
            .RequireAuthorization("StudentOnly")
            .RequireRateLimiting("issue");
        }

        private static void MapStaffEndpoints(this IEndpointRouteBuilder app)
        {
            var staffGroup = app.MapGroup("/staff").RequireAuthorization("StaffOrAdmin");

            staffGroup.MapGet("/issues", async (HttpContext ctx, AppDbContext db) =>
            {
                var userId = SecurityHelper.GetUserId(ctx, db);
                var issues = await db.Issues
                    .Where(i => i.AssignedToId == userId && !i.IsDeleted)
                    .Include(i => i.User)
                    .OrderByDescending(i => i.CreatedAt)
                    .ToListAsync();

                return Results.Ok(issues);
            });

            staffGroup.MapPut("/issues/{id}/status", async (HttpContext ctx, AppDbContext db, IValidator<UpdateStatusDto> validator, long id, UpdateStatusDto dto) =>
            {
                var validation = await validator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return Results.BadRequest(validation.Errors.Select(e => e.ErrorMessage));

                var userId = SecurityHelper.GetUserId(ctx, db);
                var issue = await db.Issues.FindAsync(id);

                if (issue == null || issue.IsDeleted) return Results.NotFound();
                if (issue.AssignedToId != userId) return Results.Forbid();

                if (issue.Status == IssueStatus.Resolved && dto.Status == IssueStatus.Open)
                    return Results.BadRequest("Cannot reopen resolved issue");

                issue.Status = dto.Status;
                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Status updated" });
            });
        }

        private static void MapStudentEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/student/issues", async (HttpContext ctx, AppDbContext db) =>
            {
                var userId = SecurityHelper.GetUserId(ctx, db);
                var issues = await db.Issues
                    .Where(i => i.UserId == userId && !i.IsDeleted)
                    .OrderByDescending(i => i.CreatedAt)
                    .ToListAsync();

                return Results.Ok(issues);
            })
            .RequireAuthorization("StudentOnly");
        }
    }
}
