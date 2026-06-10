using backend.Features.Students;
using backend.Infrastructure;
using backend.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.Students
{
    public static class StudentEndpoints
    {
        public static void MapStudentEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/admin/students").RequireAuthorization("AdminOnly");

            group.MapPost("/import", async (HttpRequest request, CsvImportService importService) =>
            {
                if (!request.HasFormContentType || request.Form.Files.Count == 0)
                    return Results.BadRequest("No file uploaded.");

                var file = request.Form.Files[0];
                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    return Results.BadRequest("Only .csv files are allowed.");

                var result = await importService.ImportStudentsAsync(file);
                return Results.Ok(result);
            }).DisableAntiforgery();

            group.MapGet("/", async (AppDbContext db, [AsParameters] StudentQueryDto query) =>
            {
                var queryable = db.Students.OrderByDescending(s => s.CreatedAt);
                
                var total = await queryable.CountAsync();
                
                var students = await queryable
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .Select(s => new StudentResponseDto
                    {
                        Id = s.Id,
                        StudentId = s.StudentId,
                        Name = s.Name,
                        Email = s.Email,
                        Gender = s.Gender.ToString(),
                        IsActive = s.IsActive,
                        CreatedAt = s.CreatedAt
                    })
                    .ToListAsync();

                return Results.Ok(new
                {
                    items = students,
                    page = query.Page,
                    pageSize = query.PageSize,
                    total = total
                });
            });

            group.MapPut("/{id}/deactivate", async (AppDbContext db, long id) =>
            {
                var student = await db.Students.FindAsync(id);
                if (student == null) return Results.NotFound();

                student.IsActive = false;
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            group.MapPut("/{id}/reset-password", async (AppDbContext db, long id) =>
            {
                var student = await db.Students.FindAsync(id);
                if (student == null) return Results.NotFound();

                student.PasswordHash = SecurityHelper.HashPassword("Student@123");
                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Password reset successfully" });
            });
        }
    }
}
