using backend.Infrastructure;
using backend.Models;
using backend.Enums;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.Auth
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/login", async (AppDbContext db, LoginDto loginUser, IConfiguration config) =>
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email);

                if (user == null || !SecurityHelper.VerifyPassword(loginUser.Password, user.PasswordHash))
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
    }
}
