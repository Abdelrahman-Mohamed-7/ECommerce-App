using ECommerce.API.Services.Interfaces;

namespace ECommerce.API.Services.Implementations
{
    public class ImageService(IWebHostEnvironment env) : IImageService
    {
        private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
        private const long MaxFileSize = 5 * 1024 * 1024;

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("No file provided.");
            if (file.Length > MaxFileSize) throw new ArgumentException("File size exceeds 5MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension)) throw new ArgumentException($"Invalid file type.");
            if (!file.ContentType.StartsWith("image/")) throw new ArgumentException("File is not an image.");

            var folder = Path.Combine(env.WebRootPath, "images", "products");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/images/products/{fileName}";
        }

        public Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl) || !imageUrl.StartsWith("/images/")) return Task.FromResult(false);
            var fullPath = Path.Combine(env.WebRootPath, imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}