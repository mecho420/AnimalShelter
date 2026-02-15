using AnimalShelter.Data;
using AnimalShelter.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace AnimalShelter.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment env;

        public AnimalService(ApplicationDbContext db, IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
        }

        public async Task<List<Animal>> GetPublicAnimalsAsync()
        {
            // Ако имаш поле/enum за статус, можеш да филтрираш тук (пример: ForAdoption)
            return await db.Animals
                .AsNoTracking()
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task<List<Animal>> GetAdminAnimalsAsync()
        {
            return await db.Animals
                .AsNoTracking()
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task<Animal?> GetByIdAsync(int id)
        {
            return await db.Animals
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<int> CreateAsync(Animal animal)
        {
            db.Animals.Add(animal);
            await db.SaveChangesAsync();
            return animal.Id;
        }

        public async Task<bool> UpdateAsync(Animal animal)
        {
            var existing = await db.Animals.FirstOrDefaultAsync(a => a.Id == animal.Id);
            if (existing == null) return false;

            // Мапване на полетата, които редактираме
            existing.Name = animal.Name;
            existing.Species = animal.Species;
            existing.Age = animal.Age;
            existing.Gender = animal.Gender;
            existing.Status = animal.Status;
            existing.Description = animal.Description;
            existing.HealthInfo = animal.HealthInfo;
            existing.ImagePath = animal.ImagePath;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await db.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (existing == null) return false;

            DeleteImageFileIfNeeded(existing.ImagePath);

            db.Animals.Remove(existing);
            await db.SaveChangesAsync();
            return true;
        }

        private void DeleteImageFileIfNeeded(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return;

            // Трием само ако е в /images/animals/ и не е default
            if (!imagePath.StartsWith("/images/animals/"))
                return;

            var fileName = Path.GetFileName(imagePath);

            // всички default имена
            var defaults = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "default-dog.jpg",
        "default-cat.jpg"
    };

            if (defaults.Contains(fileName))
                return;

            // физически път на web path
            var relative = imagePath.TrimStart('/')
                .Replace("/", Path.DirectorySeparatorChar.ToString());

            var fullPath = Path.Combine(env.WebRootPath, relative);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
