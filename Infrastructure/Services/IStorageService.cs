using Microsoft.AspNetCore.Http;

namespace backend.Infrastructure.Services
{
    public interface IStorageService
    {
        Task DeleteAsync(string objectKey, string? providerName = null, CancellationToken cancellationToken = default);
        string GetUrl(string objectKey, string? providerName = null, HttpContext? context = null);
    }
}
