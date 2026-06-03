using backend.Infrastructure;
using Microsoft.EntityFrameworkCore;

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
        }
    }
}
