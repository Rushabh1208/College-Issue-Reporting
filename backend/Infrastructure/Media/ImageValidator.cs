using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace backend.Infrastructure.Media
{
    public sealed class ImageValidator : IImageValidator
    {
        private const long MaxUploadBytes = 5 * 1024 * 1024;
        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/jpg",
            "image/png"
        };

        public async Task ValidateAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length <= 0)
            {
                throw BuildValidationException("Image file is required.");
            }

            if (file.Length > MaxUploadBytes)
            {
                throw BuildValidationException("File size exceeds 5MB limit.");
            }

            if (!AllowedContentTypes.Contains(file.ContentType ?? string.Empty))
            {
                throw BuildValidationException("Only JPEG and PNG images are allowed.");
            }

            await using var stream = file.OpenReadStream();
            var header = new byte[8];
            var bytesRead = await stream.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);

            if (!IsAllowedSignature(header.AsSpan(0, bytesRead), file.ContentType ?? string.Empty))
            {
                throw BuildValidationException("The uploaded file does not appear to be a valid JPEG or PNG image.");
            }
        }

        public string GetNormalizedExtension(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return extension switch
            {
                ".jpeg" => ".jpg",
                ".jpg" => ".jpg",
                ".png" => ".png",
                _ when string.Equals(file.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) => ".png",
                _ => ".jpg"
            };
        }

        private static bool IsAllowedSignature(ReadOnlySpan<byte> header, string contentType)
        {
            if (string.Equals(contentType, "image/png", StringComparison.OrdinalIgnoreCase))
            {
                return header.Length >= 8 &&
                       header[0] == 0x89 &&
                       header[1] == 0x50 &&
                       header[2] == 0x4E &&
                       header[3] == 0x47 &&
                       header[4] == 0x0D &&
                       header[5] == 0x0A &&
                       header[6] == 0x1A &&
                       header[7] == 0x0A;
            }

            return header.Length >= 3 &&
                   header[0] == 0xFF &&
                   header[1] == 0xD8 &&
                   header[2] == 0xFF;
        }

        private static ValidationException BuildValidationException(string message)
        {
            return new ValidationException(new[]
            {
                new ValidationFailure("Image", message)
            });
        }
    }
}
