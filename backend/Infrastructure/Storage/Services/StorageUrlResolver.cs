using backend.Infrastructure.Storage.Options;
using Microsoft.Extensions.Options;

namespace backend.Infrastructure.Storage.Services
{
    public sealed class StorageUrlResolver
    {
        private readonly StorageOptions _options;

        public StorageUrlResolver(IOptions<StorageOptions> options)
        {
            _options = options.Value;
        }

        public string Resolve(string url, HttpContext? context = null)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return url;
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                return url;
            }

            if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
            {
                return $"{_options.PublicBaseUrl.TrimEnd('/')}/{url.TrimStart('/')}";
            }

            if (context == null)
            {
                return url;
            }

            var request = context.Request;
            return $"{request.Scheme}://{request.Host}{request.PathBase}{url}";
        }
    }
}
