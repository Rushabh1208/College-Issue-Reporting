namespace backend.Infrastructure.Storage.Options
{
    public sealed class StorageOptions
    {
        public string Provider { get; set; } = "local";
        public string PublicBaseUrl { get; set; } = string.Empty;
    }
}
