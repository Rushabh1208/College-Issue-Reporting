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

            app.MapPut("/account/change-password", async (HttpContext ctx, AppDbContext db, ChangePasswordDto dto, FluentValidation.IValidator<ChangePasswordDto> validator) =>
            {
                var validation = await validator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return Results.BadRequest(new { message = string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)) });

                var userId = SecurityHelper.GetUserId(ctx);
                var userType = SecurityHelper.GetUserType(ctx);

                if (userType == "student")
                {
                    var student = await db.Students.FindAsync(userId);
                    if (student == null) return Results.NotFound();

                    if (!SecurityHelper.VerifyPassword(dto.CurrentPassword, student.PasswordHash))
                        return Results.BadRequest(new { message = "Current password is incorrect." });

                    student.PasswordHash = SecurityHelper.HashPassword(dto.NewPassword);
                    await db.SaveChangesAsync();
                }
                else
                {
                    var user = await db.Users.FindAsync(userId);
                    if (user == null) return Results.NotFound();

                    if (!SecurityHelper.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                        return Results.BadRequest(new { message = "Current password is incorrect." });

                    user.PasswordHash = SecurityHelper.HashPassword(dto.NewPassword);
                    await db.SaveChangesAsync();
                }

                return Results.Ok(new { message = "Password updated successfully." });
            })
            .RequireAuthorization();
        }
    }
}
