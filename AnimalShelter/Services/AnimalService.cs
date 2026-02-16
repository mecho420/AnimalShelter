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
        private readonly IImageService imageService;

        public AnimalService(ApplicationDbContext db, IImageService imageService)
        {
            this.db = db;
            this.imageService = imageService;
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

        public async Task<bool> UpdateAsync(Animal input, IFormFile? imageFile)
        {
            var dbAnimal = await db.Animals.FirstOrDefaultAsync(a => a.Id == input.Id);
            if (dbAnimal == null) return false;

            dbAnimal.Name = input.Name;
            dbAnimal.Species = input.Species;
            dbAnimal.Age = input.Age;
            dbAnimal.Gender = input.Gender;
            dbAnimal.Status = input.Status;
            dbAnimal.Description = input.Description;
            dbAnimal.HealthInfo = input.HealthInfo;

            if (imageFile != null && imageFile.Length > 0)
            {
                var oldImagePath = dbAnimal.ImagePath;

                var newPath = await imageService.SaveAnimalImageAsync(imageFile);
                dbAnimal.ImagePath = newPath;

                imageService.DeleteAnimalImageIfCustom(oldImagePath);
            }

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await db.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (existing == null) return false;

            imageService.DeleteAnimalImageIfCustom(existing.ImagePath);

            db.Animals.Remove(existing);
            await db.SaveChangesAsync();
            return true;
        }
    }
}
