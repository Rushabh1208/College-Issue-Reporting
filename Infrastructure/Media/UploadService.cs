using backend.Infrastructure.Storage.Abstractions;
using backend.Infrastructure.Storage.Models;

namespace backend.Infrastructure.Media
{
    public sealed class UploadService : IUploadService
    {
        private readonly IImageValidator _validator;
        private readonly IStorageProviderResolver _providerResolver;

        public UploadService(IImageValidator validator, IStorageProviderResolver providerResolver)
        {
            _validator = validator;
            _providerResolver = providerResolver;
        }

        public async Task<StorageResult> UploadIssueImageAsync(long issueId, IFormFile file, CancellationToken cancellationToken = default)
        {
            await _validator.ValidateAsync(file, cancellationToken);

            var extension = _validator.GetNormalizedExtension(file);
            var objectKey = $"issues/{issueId}/{Guid.NewGuid():N}{extension}";
            var provider = _providerResolver.GetProvider();

            await using var stream = file.OpenReadStream();
            var result = await provider.UploadAsync(stream, objectKey, file.ContentType, cancellationToken);
            result.SizeBytes = file.Length;
            result.ProviderName = provider.ProviderName;
            result.OriginalFileName = Path.GetFileName(file.FileName);
            result.Url = provider.GetFileUrl(objectKey);
            return result;
        }
    }
}
