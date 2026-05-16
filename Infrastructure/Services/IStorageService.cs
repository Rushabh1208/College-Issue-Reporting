using Microsoft.AspNetCore.Http;

namespace backend.Infrastructure.Services
{
    public interface IStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string fileName);
        Task DeleteFileAsync(string fileName);
        string GetUrl(string fileName, HttpContext context);
    }
}
