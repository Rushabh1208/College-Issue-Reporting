using Amazon.S3;
using Amazon.S3.Model;
using backend.Infrastructure.Storage.Abstractions;
using backend.Infrastructure.Storage.Models;
using backend.Infrastructure.Storage.Options;
using Microsoft.Extensions.Options;

namespace backend.Infrastructure.Storage.Providers
{
    public sealed class S3StorageProvider : IStorageProvider
    {
        private readonly IAmazonS3 _client;
        private readonly S3Options _options;

        public S3StorageProvider(IAmazonS3 client, IOptions<S3Options> options)
        {
            _client = client;
            _options = options.Value;
        }

        public string ProviderName => "s3";

        public async Task<StorageResult> UploadAsync(
            Stream stream,
            string objectKey,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.BucketName))
            {
                throw new InvalidOperationException("AWS:BucketName must be configured when Storage:Provider is s3.");
            }

            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey,
                InputStream = stream,
                ContentType = contentType,
                AutoCloseStream = false
            };

            await _client.PutObjectAsync(request, cancellationToken);

            return new StorageResult
            {
                ObjectKey = objectKey,
                ProviderName = ProviderName,
                ContentType = contentType,
                Url = GetFileUrl(objectKey)
            };
        }

        public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.BucketName))
            {
                return;
            }

            await _client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey
            }, cancellationToken);
        }

        public string GetFileUrl(string objectKey)
        {
            var normalizedKey = string.Join(
                "/",
                objectKey.Split('/', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Uri.EscapeDataString));

            if (_options.UseCloudFront && !string.IsNullOrWhiteSpace(_options.CloudFrontBaseUrl))
            {
                return $"{_options.CloudFrontBaseUrl.TrimEnd('/')}/{normalizedKey}";
            }

            if (_options.UseSignedUrls && !string.IsNullOrWhiteSpace(_options.BucketName))
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _options.BucketName,
                    Key = objectKey,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                };

                return _client.GetPreSignedURL(request);
            }

            var region = string.IsNullOrWhiteSpace(_options.Region) ? "us-east-1" : _options.Region;
            return $"https://{_options.BucketName}.s3.{region}.amazonaws.com/{normalizedKey}";
        }
    }
}
