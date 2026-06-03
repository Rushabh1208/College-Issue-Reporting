using backend.Infrastructure.Storage.Models;
using Microsoft.AspNetCore.Http;

namespace backend.Infrastructure.Media
{
    public interface IUploadService
    {
        Task<StorageResult> UploadIssueImageAsync(long issueId, IFormFile file, CancellationToken cancellationToken = default);
    }
}
