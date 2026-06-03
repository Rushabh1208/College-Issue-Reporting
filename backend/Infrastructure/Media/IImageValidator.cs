using Microsoft.AspNetCore.Http;

namespace backend.Infrastructure.Media
{
    public interface IImageValidator
    {
        Task ValidateAsync(IFormFile file, CancellationToken cancellationToken = default);
        string GetNormalizedExtension(IFormFile file);
    }
}
