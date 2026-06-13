using backend.Infrastructure;
using backend.Infrastructure.Media;
using backend.Infrastructure.Services;
using backend.Infrastructure.Storage.Models;
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
            
            adminGroup.MapGet("/issues/stats", async (AppDbContext db) =>
            {
                var counts = await db.Issues
                    .Where(i => !i.IsDeleted && (i.Category == null || !i.Category.IsWomenWelfare))
                    .GroupBy(i => i.Status)
                    .Select(g => new { Status = g.Key ?? IssueStatus.Open, Count = g.Count() })
                    .ToListAsync();

                return Results.Ok(BuildStatusCountDict(counts));
            });

            adminGroup.MapGet("/issues", async (AppDbContext db, IConnectionMultiplexer redis, IStorageService storage, HttpContext ctx, [Microsoft.AspNetCore.Mvc.FromQuery] IssueStatus? status, int page = 1, int pageSize = 10) =>
            {
                var cache = redis.GetDatabase();
                var cacheKey = $"admin:issues:{status}:{page}:{pageSize}";

                var cached = await cache.StringGetAsync(cacheKey);
                if (!cached.IsNullOrEmpty)
                    return Results.Ok(JsonSerializer.Deserialize<object>(cached!));

                var query = db.Issues
                    .Where(i => !i.IsDeleted && (i.Category == null || !i.Category.IsWomenWelfare))
                    .Include(i => i.AssignedTo)
                    .Include(i => i.Category)
                    .Include(i => i.Student)
                    .AsQueryable();

                if (status.HasValue)
                    query = query.Where(i => i.Status == status.Value);
                else
                    query = query.Where(i => i.Status == IssueStatus.Open || i.Status == IssueStatus.InProgress);

                var data = await query
                    .OrderByDescending(i => i.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = data.Select(i => new AdminIssueResponseDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    Status = i.Status ?? IssueStatus.Open,
                    Block = i.Block,
                    RoomNumber = i.RoomNumber,
                    CreatedAt = i.CreatedAt,
                    AssignedStaffName = i.AssignedTo != null ? i.AssignedTo.Name : "Unassigned",
                    ImageUrl = i.ImagePath != null ? storage.GetUrl(i.ImagePath, i.ImageStorageProvider, ctx) : null,
                    ImageObjectKey = i.ImageObjectKey ?? i.ImagePath,
                    ImageStorageProvider = i.ImageStorageProvider ?? "local",
                    ImageMimeType = i.ImageMimeType,
                    ImageSizeBytes = i.ImageSizeBytes,
                    CategoryName = i.Category != null ? i.Category.Name : string.Empty,
                    Priority = i.Priority,
                    IsAnonymous = i.IsAnonymous,
                    UpvoteCount = i.UpvoteCount,
                    ReporterName = i.IsAnonymous ? "Anonymous" : (i.Student != null ? i.Student.Name : "Unknown"),
                    ReporterStudentId = i.Student != null ? i.Student.StudentId : string.Empty
                });

                await cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(result), TimeSpan.FromMinutes(5));
                return Results.Ok(result);
            });

            adminGroup.MapPut("/issues/{id}/assign/{staffId}", async (AppDbContext db, IConnectionMultiplexer redis, IssueTimelineService timelineService, HttpContext ctx, long id, long staffId) =>
            {
                var issue = await db.Issues.FindAsync(id);
                if (issue == null || issue.IsDeleted) return Results.NotFound();

                var staff = await db.Users.FindAsync(staffId);
                if (staff == null || staff.Role != UserRole.Staff)
                    return Results.BadRequest("Invalid staff user");

                bool isReassignment = issue.AssignedToId != null;

                issue.AssignedToId = staffId;
                issue.Status = IssueStatus.InProgress;
                await db.SaveChangesAsync();

                var adminEmail = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "admin";
                await timelineService.AddTimelineEntryAsync(issue.Id, isReassignment ? "Reassigned" : "Assigned", adminEmail);

                await InvalidateAdminCache(redis);
                return Results.Ok(new { message = "Issue assigned to staff", issue.Id, issue.Status });
            });

            adminGroup.MapPut("/issues/{id}/priority", async (AppDbContext db, IConnectionMultiplexer redis, IssueTimelineService timelineService, HttpContext ctx, long id, UpdateIssuePriorityDto dto) =>
            {
                var issue = await db.Issues.FindAsync(id);
                if (issue == null || issue.IsDeleted) return Results.NotFound();

                issue.Priority = dto.Priority;
                await db.SaveChangesAsync();

                var adminEmail = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "admin";
                await timelineService.AddTimelineEntryAsync(issue.Id, "PriorityChanged", adminEmail, $"Priority changed to {dto.Priority}");

                await InvalidateAdminCache(redis);
                return Results.Ok(new { message = "Priority updated" });
            });

            adminGroup.MapPut("/issues/{id}/close", async (AppDbContext db, IConnectionMultiplexer redis, IssueTimelineService timelineService, HttpContext ctx, long id) =>
            {
                var issue = await db.Issues.FindAsync(id);
                if (issue == null || issue.IsDeleted) return Results.NotFound();

                issue.Status = IssueStatus.Closed;
                await db.SaveChangesAsync();

                var adminEmail = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "admin";
                await timelineService.AddTimelineEntryAsync(issue.Id, "Closed", adminEmail);

                await InvalidateAdminCache(redis);
                return Results.Ok(new { message = "Issue closed" });
            });

            adminGroup.MapDelete("/issues/{id}", async (AppDbContext db, IConnectionMultiplexer redis, IStorageService storage, long id) =>
            {
                var issue = await db.Issues.FindAsync(id);
                if (issue == null || issue.IsDeleted) return Results.NotFound();

                issue.IsDeleted = true;
                if (!string.IsNullOrEmpty(issue.ImagePath))
                {
                    await storage.DeleteAsync(issue.ImagePath, issue.ImageStorageProvider);
                    issue.ImagePath = null;
                    issue.ImageObjectKey = null;
                    issue.ImageStorageProvider = null;
                    issue.ImageMimeType = null;
                    issue.ImageSizeBytes = null;
                }
                
                await db.SaveChangesAsync();

                await InvalidateAdminCache(redis);
                return Results.Ok(new { message = "Issue deleted successfully" });
            });

            app.MapPost("/issues/report", async (HttpContext ctx, AppDbContext db, IConnectionMultiplexer redis, IStorageService storage, IUploadService uploadService, IValidator<CreateIssueDto> validator, IssueTimelineService timelineService) =>
            {
                var form = await ctx.Request.ReadFormAsync();
                
                var dto = new CreateIssueDto
                {
                    Title = form["Title"].ToString(),
                    Description = form["Description"].ToString(),
                    Block = form["Block"].ToString(),
                    RoomNumber = form["RoomNumber"].ToString(),
                    CategoryId = int.TryParse(form["CategoryId"], out var catId) ? catId : 0,
                    Priority = Enum.TryParse<IssuePriority>(form["Priority"], out var p) ? p : IssuePriority.Medium,
                    IsAnonymous = bool.TryParse(form["IsAnonymous"], out var anon) ? anon : false
                };

                var validation = await validator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return Results.BadRequest(validation.Errors.Select(e => e.ErrorMessage));

                var categoryExists = await db.IssueCategories.AnyAsync(c => c.Id == dto.CategoryId && c.IsActive);
                if (!categoryExists)
                    return Results.BadRequest(new { message = "Invalid category selected." });

                var studentId = SecurityHelper.GetUserId(ctx);
                var student = await db.Students.FirstOrDefaultAsync(x => x.Id == studentId);
                if (student == null)
                    return Results.Json(new { message = "Student account not found." }, statusCode: 401);

                var duplicateIssue = await db.Issues
                    .FirstOrDefaultAsync(x =>
                        x.CategoryId == dto.CategoryId &&
                        x.Block == dto.Block &&
                        x.RoomNumber == dto.RoomNumber &&
                        !x.IsDeleted &&
                        x.Status != IssueStatus.Resolved);

                if (duplicateIssue != null)
                {
                    return Results.Conflict(new
                    {
                        duplicateIssueId = duplicateIssue.Id,
                        message = "Similar issue already exists"
                    });
                }

                var issue = new Issue
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Block = dto.Block,
                    RoomNumber = dto.RoomNumber,
                    CategoryId = dto.CategoryId,
                    Priority = dto.Priority,
                    IsAnonymous = dto.IsAnonymous,
                    StudentId = studentId,
                    Status = IssueStatus.Open,
                    CreatedAt = DateTime.UtcNow
                };

                db.Issues.Add(issue);
                await db.SaveChangesAsync(); // Save first to get ID

                await timelineService.AddTimelineEntryAsync(
                    issue.Id,
                    "Created",
                    student.StudentId);

                var image = form.Files.GetFile("Image");
                if (image != null)
                {
                    try
                    {
                        var upload = await uploadService.UploadIssueImageAsync(issue.Id, image);
                        issue.ImagePath = upload.ObjectKey;
                        issue.ImageObjectKey = upload.ObjectKey;
                        issue.ImageStorageProvider = upload.ProviderName;
                        issue.ImageMimeType = upload.ContentType;
                        issue.ImageSizeBytes = upload.SizeBytes;
                        await db.SaveChangesAsync();
                    }
                    catch (ValidationException ex)
                    {
                        db.Issues.Remove(issue);
                        await db.SaveChangesAsync();
                        return Results.BadRequest(ex.Errors.Select(e => e.ErrorMessage));
                    }
                    catch
                    {
                        db.Issues.Remove(issue);
                        await db.SaveChangesAsync();
                        throw;
                    }
                }

                var category = await db.IssueCategories.FirstOrDefaultAsync(x => x.Id == issue.CategoryId);
                if (category == null || !category.IsWomenWelfare)
                {
                    await InvalidateAdminCache(redis);
                }
                var imageUrl = issue.ImagePath != null ? storage.GetUrl(issue.ImagePath, issue.ImageStorageProvider, ctx) : null;
                var responseIssue = new IssueResponseDto
                {
                    Id = issue.Id,
                    Title = issue.Title,
                    Description = issue.Description,
                    Status = issue.Status ?? IssueStatus.Open,
                    Block = issue.Block,
                    RoomNumber = issue.RoomNumber,
                    CreatedAt = issue.CreatedAt,
                    AssignedStaffName = "Unassigned",
                    ImageUrl = imageUrl,
                    ImageObjectKey = issue.ImageObjectKey ?? issue.ImagePath,
                    ImageStorageProvider = issue.ImageStorageProvider ?? "local",
                    ImageMimeType = issue.ImageMimeType,
                    ImageSizeBytes = issue.ImageSizeBytes,
                    CategoryName = await db.IssueCategories.Where(c => c.Id == issue.CategoryId).Select(c => c.Name).FirstOrDefaultAsync() ?? string.Empty,
                    Priority = issue.Priority,
                    IsAnonymous = issue.IsAnonymous,
                    UpvoteCount = issue.UpvoteCount
                };

                return Results.Ok(new
                {
                    message = "Issue created",
                    issue = responseIssue,
                    imageUrl,
                    image = issue.ImagePath == null ? null : new
                    {
                        url = imageUrl,
                        provider = issue.ImageStorageProvider ?? "local",
                        mimeType = issue.ImageMimeType,
                        sizeBytes = issue.ImageSizeBytes
                    }
                });
            })
            .RequireAuthorization("StudentOnly")
            .RequireRateLimiting("issue");

            app.MapPost("/issues/{id}/upvote", async (long id, HttpContext ctx, AppDbContext db) =>
            {
                var studentId = SecurityHelper.GetUserId(ctx);
                var issue = await db.Issues.FirstOrDefaultAsync(x => x.Id == id);
                if (issue == null) return Results.NotFound();

                if (issue.StudentId == studentId)
                {
                    return Results.BadRequest(new { message = "You cannot upvote your own issue." });
                }

                if (issue.Status == IssueStatus.Resolved || issue.Status == IssueStatus.Closed)
                {
                    return Results.BadRequest(new { message = "Cannot upvote a resolved or closed issue." });
                }

                var existingVote = await db.IssueUpvotes.FirstOrDefaultAsync(x => x.IssueId == id && x.StudentId == studentId);
                if (existingVote != null)
                {
                    return Results.Conflict(new { message = "You have already upvoted this issue." });
                }

                db.IssueUpvotes.Add(new IssueUpvote
                {
                    IssueId = id,
                    StudentId = studentId,
                    CreatedAt = DateTime.UtcNow
                });

                issue.UpvoteCount++;

                var student = await db.Students.FirstOrDefaultAsync(x => x.Id == studentId);
                if (student != null)
                {
                    db.IssueTimelines.Add(new IssueTimeline
                    {
                        IssueId = id,
                        Action = TimelineAction.Upvoted,
                        PerformedBy = student.StudentId,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await db.SaveChangesAsync();

                return Results.Ok(new { upvoteCount = issue.UpvoteCount });
            }).RequireAuthorization("StudentOnly");

            app.MapDelete("/issues/{id}/upvote", async (long id, HttpContext ctx, AppDbContext db) =>
            {
                var studentId = SecurityHelper.GetUserId(ctx);
                var issue = await db.Issues.FirstOrDefaultAsync(x => x.Id == id);
                if (issue == null) return Results.NotFound();

                var vote = await db.IssueUpvotes.FirstOrDefaultAsync(x => x.IssueId == id && x.StudentId == studentId);
                if (vote == null) return Results.NotFound();

                db.IssueUpvotes.Remove(vote);
                issue.UpvoteCount = Math.Max(0, issue.UpvoteCount - 1);

                await db.SaveChangesAsync();

                return Results.Ok(new { upvoteCount = issue.UpvoteCount });
            }).RequireAuthorization("StudentOnly");

            var staffGroup = app.MapGroup("/staff").RequireAuthorization("StaffOrAdmin");

            staffGroup.MapGet("/issues/stats", async (HttpContext ctx, AppDbContext db) =>
            {
                var userId = SecurityHelper.GetUserId(ctx);
                var counts = await db.Issues
                    .Where(i => !i.IsDeleted && i.AssignedToId == userId)
                    .GroupBy(i => i.Status)
                    .Select(g => new { Status = g.Key ?? IssueStatus.Open, Count = g.Count() })
                    .ToListAsync();

                return Results.Ok(BuildStatusCountDict(counts));
            });

            staffGroup.MapGet("/issues", async (HttpContext ctx, AppDbContext db, IStorageService storage, [Microsoft.AspNetCore.Mvc.FromQuery] IssueStatus? status) =>
            {
                var userId = SecurityHelper.GetUserId(ctx);
                var query = db.Issues
                    .Where(i => i.AssignedToId == userId && !i.IsDeleted)
                    .Include(i => i.Category)
                    .AsQueryable();

                if (status.HasValue)
                    query = query.Where(i => i.Status == status.Value);
                else
                    query = query.Where(i => i.Status == IssueStatus.Open || i.Status == IssueStatus.InProgress);

                var issues = await query
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
                        ImageUrl = i.ImagePath != null ? storage.GetUrl(i.ImagePath, i.ImageStorageProvider, ctx) : null,
                        ImageObjectKey = i.ImageObjectKey ?? i.ImagePath,
                        ImageStorageProvider = i.ImageStorageProvider ?? "local",
                        ImageMimeType = i.ImageMimeType,
                        ImageSizeBytes = i.ImageSizeBytes,
                        CategoryName = i.Category != null ? i.Category.Name : string.Empty,
                        Priority = i.Priority,
                        IsAnonymous = i.IsAnonymous,
                        UpvoteCount = i.UpvoteCount
                    })
                    .ToListAsync();

                return Results.Ok(issues);
            });

            staffGroup.MapPut("/issues/{id}/status", async (HttpContext ctx, AppDbContext db, IConnectionMultiplexer redis, IValidator<UpdateStatusDto> validator, IssueTimelineService timelineService, long id, UpdateStatusDto dto) =>
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

                var staffEmail = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "staff";
                await timelineService.AddTimelineEntryAsync(issue.Id, "StatusChanged", staffEmail, $"Status changed to {issue.Status}");

                if (issue.Status == IssueStatus.Resolved)
                {
                    await timelineService.AddTimelineEntryAsync(issue.Id, "Resolved", staffEmail);
                }

                await InvalidateAdminCache(redis);
                return Results.Ok(new { message = "Status updated" });
            });

            app.MapGet("/student/issues/stats", async (HttpContext ctx, AppDbContext db) =>
            {
                var studentId = SecurityHelper.GetUserId(ctx);
                var counts = await db.Issues
                    .Where(i => i.StudentId == studentId && !i.IsDeleted)
                    .GroupBy(i => i.Status)
                    .Select(g => new { Status = g.Key ?? IssueStatus.Open, Count = g.Count() })
                    .ToListAsync();

                return Results.Ok(BuildStatusCountDict(counts));
            }).RequireAuthorization("StudentOnly");

            app.MapGet("/student/issues", async (HttpContext ctx, AppDbContext db, IStorageService storage, [Microsoft.AspNetCore.Mvc.FromQuery] IssueStatus? status, bool all = false) =>
            {
                var studentId = SecurityHelper.GetUserId(ctx);
                var query = db.Issues
                    .Where(i => i.StudentId == studentId && !i.IsDeleted)
                    .Include(i => i.Category)
                    .Include(i => i.IssueUpvotes)
                    .AsQueryable();

                if (status.HasValue)
                {
                    query = query.Where(i => i.Status == status.Value);
                }
                else if (!all)
                {
                    query = query.Where(i => i.Status == IssueStatus.Open || i.Status == IssueStatus.InProgress);
                }

                var issues = await query
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
                        ImageUrl = i.ImagePath != null ? storage.GetUrl(i.ImagePath, i.ImageStorageProvider, ctx) : null,
                        ImageObjectKey = i.ImageObjectKey ?? i.ImagePath,
                        ImageStorageProvider = i.ImageStorageProvider ?? "local",
                        ImageMimeType = i.ImageMimeType,
                        ImageSizeBytes = i.ImageSizeBytes,
                        CategoryName = i.Category != null ? i.Category.Name : string.Empty,
                        Priority = i.Priority,
                        IsAnonymous = i.IsAnonymous,
                        UpvoteCount = i.UpvoteCount,
                        HasUpvoted = i.IssueUpvotes.Any(x => x.StudentId == studentId),
                        IsOwnIssue = true
                    })
                    .ToListAsync();

                return Results.Ok(issues);
            })
            .RequireAuthorization("StudentOnly");

            app.MapGet("/community/issues/stats", async (HttpContext ctx, AppDbContext db) =>
            {
                var counts = await db.Issues
                    .Where(i => !i.IsDeleted && !i.IsAnonymous && (i.Category == null || !i.Category.IsWomenWelfare)
                             && (i.Status == IssueStatus.Open || i.Status == IssueStatus.InProgress))
                    .GroupBy(i => i.Status)
                    .Select(g => new { Status = g.Key ?? IssueStatus.Open, Count = g.Count() })
                    .ToListAsync();

                return Results.Ok(BuildStatusCountDict(counts));
            })
            .RequireAuthorization("StudentOnly");

            app.MapGet("/community/issues", async (HttpContext ctx, AppDbContext db, IStorageService storage, [Microsoft.AspNetCore.Mvc.FromQuery] IssueStatus? status, string? search) =>
            {
                var studentId = SecurityHelper.GetUserId(ctx);

                var query = db.Issues
                    .Where(i => !i.IsDeleted
                             && !i.IsAnonymous
                             && (i.Category == null || !i.Category.IsWomenWelfare))
                    .Include(i => i.AssignedTo)
                    .Include(i => i.Category)
                    .Include(i => i.Student)
                    .Include(i => i.IssueUpvotes)
                    .AsQueryable();

                if (status.HasValue)
                {
                    query = query.Where(i => i.Status == status.Value);
                }
                else
                {
                    query = query.Where(i => i.Status == IssueStatus.Open || i.Status == IssueStatus.InProgress);
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var term = search.Trim().ToLower();
                    query = query.Where(i =>
                        i.Title.ToLower().Contains(term) ||
                        i.Description.ToLower().Contains(term) ||
                        i.Block.ToLower().Contains(term) ||
                        i.RoomNumber.ToLower().Contains(term));
                }

                var issues = await query
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
                        ImageUrl = i.ImagePath != null ? storage.GetUrl(i.ImagePath, i.ImageStorageProvider, ctx) : null,
                        ImageObjectKey = i.ImageObjectKey ?? i.ImagePath,
                        ImageStorageProvider = i.ImageStorageProvider ?? "local",
                        ImageMimeType = i.ImageMimeType,
                        ImageSizeBytes = i.ImageSizeBytes,
                        CategoryName = i.Category != null ? i.Category.Name : string.Empty,
                        Priority = i.Priority,
                        IsAnonymous = i.IsAnonymous,
                        UpvoteCount = i.UpvoteCount,
                        HasUpvoted = i.IssueUpvotes.Any(x => x.StudentId == studentId),
                        IsOwnIssue = i.StudentId == studentId
                    })
                    .ToListAsync();

                return Results.Ok(issues);
            })
            .RequireAuthorization("StudentOnly");

            app.MapGet("/issues/{id}/timeline", async (long id, AppDbContext db, HttpContext ctx) =>
            {
                var issue = await db.Issues.FindAsync(id);
                if (issue == null || issue.IsDeleted) return Results.NotFound();

                var role = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                var userId = SecurityHelper.GetUserId(ctx);

                if (role == "Student" && issue.StudentId != userId)
                    return Results.Forbid();

                if (role == "Staff" && issue.AssignedToId != userId)
                    return Results.Forbid();

                if (role == "WomenCell")
                {
                    await db.Entry(issue).Reference(i => i.Category).LoadAsync();
                    if (issue.Category == null || !issue.Category.IsWomenWelfare)
                        return Results.Forbid();
                }

                var timelines = await db.IssueTimelines
                    .Where(x => x.IssueId == id)
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => new TimelineResponseDto
                    {
                        Action = x.Action.ToString(),
                        PerformedBy = x.PerformedBy,
                        Notes = x.Notes,
                        CreatedAt = x.CreatedAt
                    })
                    .ToListAsync();

                return Results.Ok(timelines);
            }).RequireAuthorization();

            var womenCellGroup = app.MapGroup("/womencell").RequireAuthorization("WomenCellOnly");

            womenCellGroup.MapGet("/issues/stats", async (AppDbContext db) =>
            {
                var counts = await db.Issues
                    .Where(i => !i.IsDeleted && i.Category != null && i.Category.IsWomenWelfare)
                    .GroupBy(i => i.Status)
                    .Select(g => new { Status = g.Key ?? IssueStatus.Open, Count = g.Count() })
                    .ToListAsync();

                return Results.Ok(BuildStatusCountDict(counts));
            });

            womenCellGroup.MapGet("/issues", async (
                HttpContext ctx, AppDbContext db, IStorageService storage,
                [Microsoft.AspNetCore.Mvc.FromQuery] IssueStatus? status, int? categoryId, IssuePriority? priority,
                DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10) =>
            {
                var query = db.Issues
                    .Where(i => !i.IsDeleted && i.Category != null && i.Category.IsWomenWelfare)
                    .Include(i => i.AssignedTo)
                    .Include(i => i.Category)
                    .AsQueryable();

                if (status.HasValue) query = query.Where(i => i.Status == status.Value);
                else query = query.Where(i => i.Status == IssueStatus.Open || i.Status == IssueStatus.InProgress);

                if (categoryId.HasValue) query = query.Where(i => i.CategoryId == categoryId.Value);
                if (priority.HasValue) query = query.Where(i => i.Priority == priority.Value);
                if (fromDate.HasValue) query = query.Where(i => i.CreatedAt >= fromDate.Value);
                if (toDate.HasValue) query = query.Where(i => i.CreatedAt <= toDate.Value);

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
                    AssignedStaffName = "Women Cell",
                    ImageUrl = i.ImagePath != null ? storage.GetUrl(i.ImagePath, i.ImageStorageProvider, ctx) : null,
                    ImageObjectKey = i.ImageObjectKey ?? i.ImagePath,
                    ImageStorageProvider = i.ImageStorageProvider ?? "local",
                    ImageMimeType = i.ImageMimeType,
                    ImageSizeBytes = i.ImageSizeBytes,
                    CategoryName = i.Category != null ? i.Category.Name : string.Empty,
                    Priority = i.Priority,
                    IsAnonymous = i.IsAnonymous,
                    UpvoteCount = i.UpvoteCount
                });

                return Results.Ok(result);
            });

            womenCellGroup.MapPut("/issues/{id}/status", async (HttpContext ctx, AppDbContext db, IValidator<UpdateStatusDto> validator, IssueTimelineService timelineService, long id, UpdateStatusDto dto) =>
            {
                var validation = await validator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return Results.BadRequest(validation.Errors.Select(e => e.ErrorMessage));

                var issue = await db.Issues.Include(i => i.Category).FirstOrDefaultAsync(i => i.Id == id);

                if (issue == null || issue.IsDeleted) return Results.NotFound();
                if (issue.Category == null || !issue.Category.IsWomenWelfare) return Results.Forbid();

                if (issue.Status == IssueStatus.Resolved && dto.Status == IssueStatus.Open)
                    return Results.BadRequest("Cannot reopen resolved issue");

                issue.Status = dto.Status;
                await db.SaveChangesAsync();

                var staffEmail = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "womencell";
                await timelineService.AddTimelineEntryAsync(issue.Id, "StatusChanged", staffEmail, $"Status changed to {issue.Status}");

                if (issue.Status == IssueStatus.Resolved)
                {
                    await timelineService.AddTimelineEntryAsync(issue.Id, "Resolved", staffEmail);
                }

                return Results.Ok(new { message = "Status updated" });
            });
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

        private static Dictionary<string, int> BuildStatusCountDict(IEnumerable<dynamic> counts)
        {
            var dict = new Dictionary<string, int> { ["Open"] = 0, ["InProgress"] = 0, ["Resolved"] = 0, ["Closed"] = 0 };
            foreach (var c in counts)
            {
                var status = (IssueStatus?)c.Status ?? IssueStatus.Open;
                dict[status.ToString()] = c.Count;
            }
            return dict;
        }
    }
}
