using backend.Infrastructure.Services;
using backend.Infrastructure.Storage.Abstractions;

namespace backend.Infrastructure.Storage.Services
{
    public sealed class StorageService : IStorageService
    {
        private readonly IStorageProviderResolver _providerResolver;
        private readonly StorageUrlResolver _urlResolver;

        public StorageService(IStorageProviderResolver providerResolver, StorageUrlResolver urlResolver)
        {
            _providerResolver = providerResolver;
            _urlResolver = urlResolver;
        }

        public async Task DeleteAsync(string objectKey, string? providerName = null, CancellationToken cancellationToken = default)
        {
            var provider = _providerResolver.GetProvider(providerName);
            await provider.DeleteAsync(objectKey, cancellationToken);
        }

        public string GetUrl(string objectKey, string? providerName = null, HttpContext? context = null)
        {
            var provider = _providerResolver.GetProvider(providerName);
            var resolvedUrl = provider.GetFileUrl(objectKey);
            return _urlResolver.Resolve(resolvedUrl, context);
        }
    }
}
