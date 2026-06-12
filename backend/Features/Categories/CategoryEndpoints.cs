using System.Security.Claims;
using backend.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace backend.Features.Categories
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/categories", async (AppDbContext db, IMemoryCache cache, HttpContext ctx) =>
            {
                const string cacheKey = "IssueCategories";

                if (!cache.TryGetValue(cacheKey, out List<CategoryResponseDto>? categories))
                {
                    var dbCategories = await db.IssueCategories
                        .Where(c => c.IsActive)
                        .ToListAsync();

                    categories = dbCategories
                        .GroupBy(c => c.ParentCategory)
                        .Select(g => new CategoryResponseDto
                        {
                            ParentCategory = g.Key.ToString(),
                            Categories = g.Select(c => new CategoryItemDto
                            {
                                Id = c.Id,
                                Name = c.Name,
                                IsWomenWelfare = c.IsWomenWelfare
                            }).OrderBy(c => c.Name).ToList()
                        })
                        .OrderBy(r => r.ParentCategory)
                        .ToList();

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromDays(7));

                    cache.Set(cacheKey, categories, cacheEntryOptions);
                }

                // Filter out WomenWelfare categories if the user is a male student
                var user = ctx.User;
                var genderClaim = user?.FindFirst(System.Security.Claims.ClaimTypes.Gender)?.Value;

                if (genderClaim != null && genderClaim.Equals("Male", StringComparison.OrdinalIgnoreCase))
                {
                    var filteredCategories = categories!
                        .Select(g => new CategoryResponseDto
                        {
                            ParentCategory = g.ParentCategory,
                            Categories = g.Categories.Where(c => !c.IsWomenWelfare).ToList()
                        })
                        .Where(g => g.Categories.Any())
                        .ToList();

                    return Results.Ok(filteredCategories);
                }

                return Results.Ok(categories);
            })
            .WithTags("Categories")
            .RequireAuthorization();
        }
    }
}
