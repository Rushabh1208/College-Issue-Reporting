using backend.Infrastructure;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.Auth
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/login", async (AppDbContext db, LoginDto dto, IConfiguration config) =>
            {
                // normalize input — could be studentId like "2025001" or email
                var input = dto.Identifier.Trim().ToLowerInvariant();

                // check Students table first
                // students can log in with their StudentId OR their college email
                var student = await db.Students
                    .FirstOrDefaultAsync(s =>
                        (s.StudentId.ToLower() == input || s.Email.ToLower() == input)
                        && s.IsActive);

                if (student != null)
                {
                    if (!SecurityHelper.VerifyPassword(dto.Password, student.PasswordHash))
                        return Results.Unauthorized();

                    return Results.Ok(new
                    {
                        token = SecurityHelper.GenerateToken(student, config),
                        role = "Student",
                        name = student.Name,
                        email = student.Email,
                        studentId = student.StudentId
                    });
                }

                // check Users table (Admin, Staff, WomenCell)
                var user = await db.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == input);

                if (user == null || !SecurityHelper.VerifyPassword(dto.Password, user.PasswordHash))
                    return Results.Unauthorized();

                return Results.Ok(new
                {
                    token = SecurityHelper.GenerateToken(user, config),
                    role = user.Role.ToString(),
                    name = user.Name,
                    email = user.Email
                });
            })
            .RequireRateLimiting("login");
        }
    }
}
