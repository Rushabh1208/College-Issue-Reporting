namespace backend.Infrastructure.Storage.Abstractions
{
    public interface IStorageProviderResolver
    {
        IStorageProvider GetProvider(string? providerName = null);
        string GetDefaultProviderName();
    }
}
