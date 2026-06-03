namespace backend.Infrastructure.Storage.Models
{
    public sealed class StorageResult
    {
        public string ObjectKey { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
    }
}
