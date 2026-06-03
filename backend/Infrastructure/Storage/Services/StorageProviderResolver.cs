using backend.Infrastructure.Storage.Abstractions;
using backend.Infrastructure.Storage.Options;
using Microsoft.Extensions.Options;

namespace backend.Infrastructure.Storage.Services
{
    public sealed class StorageProviderResolver : IStorageProviderResolver
    {
        private readonly IReadOnlyDictionary<string, IStorageProvider> _providers;
        private readonly StorageOptions _options;

        public StorageProviderResolver(IEnumerable<IStorageProvider> providers, IOptions<StorageOptions> options)
        {
            _providers = providers.ToDictionary(p => p.ProviderName, StringComparer.OrdinalIgnoreCase);
            _options = options.Value;
        }

        public IStorageProvider GetProvider(string? providerName = null)
        {
            var resolvedName = string.IsNullOrWhiteSpace(providerName)
                ? GetDefaultProviderName()
                : providerName.Trim();

            if (_providers.TryGetValue(resolvedName, out var provider))
            {
                return provider;
            }

            throw new InvalidOperationException($"Storage provider '{resolvedName}' is not registered.");
        }

        public string GetDefaultProviderName()
        {
            return string.IsNullOrWhiteSpace(_options.Provider)
                ? "s3"
                : _options.Provider.Trim();
        }
    }
}
