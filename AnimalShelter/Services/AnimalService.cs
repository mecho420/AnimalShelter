using AnimalShelter.Data;
using AnimalShelter.Common;
using AnimalShelter.Models.Enums;
using AnimalShelter.ViewModels;
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

        public async Task<int> CreateAsync(Animal animal, IFormFile? imageFile)
        {
            // Снимка
            if (imageFile != null && imageFile.Length > 0)
            {
                var imagePath = await imageService.SaveAnimalImageAsync(imageFile);
                animal.ImagePath = imagePath;
            }
            else
            {
                animal.ImagePath = "/images/animals/default-dog.jpg";
            }

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
            var animal = await db.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null) return false;

            var hasRequests = await db.AdoptionRequests.AnyAsync(r => r.AnimalId == id);
            if (hasRequests)
                throw new InvalidOperationException("Не може да се изтрие животно, за което има подадени заявки.");

            imageService.DeleteAnimalImageIfCustom(animal.ImagePath);

            db.Animals.Remove(animal);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<Animal>> GetFilteredAnimalsAsync(AnimalFilterModel filter)
        {
            var query = db.Animals.AsNoTracking().AsQueryable();

            // Търсене по име
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(a => a.Name.ToLower().Contains(term));
            }

            // Филтър по вид
            if (!string.IsNullOrWhiteSpace(filter.Species))
            {
                query = query.Where(a => a.Species == filter.Species);
            }

            // Филтър по пол
            if (filter.Gender.HasValue)
            {
                query = query.Where(a => a.Gender == filter.Gender.Value);
            }

            // Филтър по статус
            if (filter.Status.HasValue)
            {
                query = query.Where(a => a.Status == filter.Status.Value);
            }

            // Филтър по години (диапазон)
            if (filter.MinAge.HasValue)
            {
                query = query.Where(a => a.Age >= filter.MinAge.Value);
            }

            if (filter.MaxAge.HasValue)
            {
                query = query.Where(a => a.Age <= filter.MaxAge.Value);
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Animal>
            {
                Items = items,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalItems = totalItems
            };
        }

        public async Task<PagedResult<Animal>> GetAdminAnimalsAsync(AnimalFilterModel filter)
        {
            var query = db.Animals.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(a => a.Name.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(filter.Species))
            {
                query = query.Where(a => a.Species == filter.Species);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(a => a.Status == filter.Status.Value);
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Animal>
            {
                Items = items,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalItems = totalItems
            };
        }
    }
}
