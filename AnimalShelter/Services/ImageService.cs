using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace AnimalShelter.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveAnimalImageAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "animals");
            Directory.CreateDirectory(uploadsFolder);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext))
                throw new InvalidOperationException("Позволени са само .jpg, .jpeg, .png, .webp");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Връщаме web път (за ImagePath)
            return $"/images/animals/{fileName}";
        }

        public void DeleteAnimalImageIfCustom(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return;

            if (!imagePath.StartsWith("/images/animals/"))
                return;

            var fileName = Path.GetFileName(imagePath);

            var defaults = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "default-dog.jpg",
                "default-cat.jpg"
            };

            if (defaults.Contains(fileName))
                return;

            var relative = imagePath.TrimStart('/')
                .Replace("/", Path.DirectorySeparatorChar.ToString());

            var fullPath = Path.Combine(_env.WebRootPath, relative);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
