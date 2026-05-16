using Microsoft.AspNetCore.Http;

namespace backend.Infrastructure.Services
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _uploadFolder;

        public LocalStorageService(IWebHostEnvironment env)
        {
            _uploadFolder = Path.Combine(env.ContentRootPath, "uploads");
            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string fileName)
        {
            var filePath = Path.Combine(_uploadFolder, fileName);
            
            // Requirements: replace existing image safely
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public Task DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_uploadFolder, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return Task.CompletedTask;
        }

        public string GetUrl(string fileName, HttpContext context)
        {
            var request = context.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            return $"{baseUrl}/uploads/{fileName}";
        }
    }
}
