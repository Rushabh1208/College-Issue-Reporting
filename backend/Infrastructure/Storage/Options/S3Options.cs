namespace backend.Infrastructure.Storage.Options
{
    public sealed class S3Options
    {
        public string BucketName { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string CloudFrontBaseUrl { get; set; } = string.Empty;
        public bool UseCloudFront { get; set; }
        public bool UseSignedUrls { get; set; }
    }
}
