using backend.Models;
using backend.Enums;
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
            app.MapGet("/", () => "Campus API is running...");
        }

        private static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/login", async (AppDbContext db, User loginUser, IConfiguration config) =>
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email);

                if (user == null || user.PasswordHash != SecurityHelper.HashPassword(loginUser.PasswordHash))
                    return Results.Unauthorized();

                return Results.Ok(new { token = SecurityHelper.GenerateToken(user, config) });
            });

            app.MapPost("/register", async (AppDbContext db, User dto) =>
            {
                if (await db.Users.AnyAsync(u => u.Email == dto.Email))
                    return Results.BadRequest("Email already registered");

                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    PasswordHash = SecurityHelper.HashPassword(dto.PasswordHash),
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
                    .AsQueryable();

                if (status.HasValue)
                    query = query.Where(i => i.Status == status.Value);

                var data = await query
                    .OrderByDescending(i => i.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = data.Select(i => new Issue
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    Status = i.Status,
                    Block = i.Block,
                    RoomNumber = i.RoomNumber,
                    CreatedAt = i.CreatedAt
                });

                await cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(result), TimeSpan.FromSeconds(60));
                return Results.Ok(result);
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


    }
}
