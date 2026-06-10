using backend.Infrastructure;
using backend.Models;
using backend.Enums;
using backend.Features.Users.Validators;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace backend.Features.Users
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var adminGroup = app.MapGroup("/admin").RequireAuthorization("AdminOnly");

            adminGroup.MapGet("/users", async (AppDbContext db, int page = 1, int pageSize = 10) =>
            {
                var users = await db.Users
                    .OrderBy(u => u.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserResponseDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        Role = u.Role
                    })
                    .ToListAsync();

                return Results.Ok(users);
            });

            var staffGroup = app.MapGroup("/admin/staff").RequireAuthorization("AdminOnly");

            staffGroup.MapPost("/", async (AppDbContext db, IValidator<CreateStaffDto> validator, CreateStaffDto dto) =>
            {
                var validationResult = await validator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                    return Results.BadRequest(validationResult.Errors);

                var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
                if (await db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail))
                    return Results.BadRequest(new { message = "Email is already in use." });

                var user = new User
                {
                    Name = dto.Name.Trim(),
                    Email = normalizedEmail,
                    Role = UserRole.Staff,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    PasswordHash = SecurityHelper.HashPassword("Staff@123")
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Staff account created successfully" });
            });

            staffGroup.MapPut("/{id}", async (AppDbContext db, long id, UpdateStaffDto dto) =>
            {
                var user = await db.Users.FindAsync(id);
                if (user == null || user.Role != UserRole.Staff)
                    return Results.NotFound();

                var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
                if (await db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail && u.Id != id))
                    return Results.BadRequest(new { message = "Email is already in use." });

                user.Name = dto.Name.Trim();
                user.Email = normalizedEmail;

                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            staffGroup.MapPut("/{id}/deactivate", async (AppDbContext db, long id) =>
            {
                var user = await db.Users.FindAsync(id);
                if (user == null || user.Role != UserRole.Staff)
                    return Results.NotFound();

                user.IsActive = false;
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            staffGroup.MapPut("/{id}/reset-password", async (AppDbContext db, long id) =>
            {
                var user = await db.Users.FindAsync(id);
                if (user == null || user.Role != UserRole.Staff)
                    return Results.NotFound();

                user.PasswordHash = SecurityHelper.HashPassword("Staff@123");
                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Password reset successfully" });
            });

            staffGroup.MapGet("/", async (AppDbContext db, [AsParameters] StaffQueryDto query) =>
            {
                var queryable = db.Users.Where(u => u.Role == UserRole.Staff).OrderByDescending(u => u.CreatedAt);
                
                var total = await queryable.CountAsync();
                
                var staff = await queryable
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .Select(u => new StaffResponseDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt
                    })
                    .ToListAsync();

                return Results.Ok(new
                {
                    items = staff,
                    page = query.Page,
                    pageSize = query.PageSize,
                    total = total
                });
            });
        }
    }
}
