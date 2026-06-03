using backend.Infrastructure.Storage.Models;

namespace backend.Infrastructure.Storage.Abstractions
{
    public interface IStorageProvider
    {
        string ProviderName { get; }

        Task<StorageResult> UploadAsync(
            Stream stream,
            string objectKey,
            string contentType,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string objectKey,
            CancellationToken cancellationToken = default);

        string GetFileUrl(string objectKey);
    }
}
