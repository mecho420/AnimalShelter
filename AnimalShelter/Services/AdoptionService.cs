using AnimalShelter.Data;
using AnimalShelter.Models;
using AnimalShelter.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalShelter.Services
{
    public class AdoptionService : IAdoptionService
    {
        private readonly ApplicationDbContext db;

        public AdoptionService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<Animal?> GetAnimalForAdoptionRequestAsync(int animalId)
        {
            var animal = await db.Animals.FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal == null) return null;

            // ако не е за осиновяване, просто връщаме животното
            // PageModel-а ще реши дали да редиректне
            return animal;
        }

        public async Task<bool> HasUserAlreadyRequestedAsync(int animalId, string userId)
        {
            return await db.AdoptionRequests.AnyAsync(r => r.AnimalId == animalId && r.UserId == userId);
        }

        public async Task<int> CreateRequestAsync(int animalId, string userId, string fullName, string phone, string email, string? comment)
        {
            var animal = await db.Animals.FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal == null)
                throw new InvalidOperationException("Животното не беше намерено.");

            if (animal.Status != AnimalStatus.ForAdoption)
                throw new InvalidOperationException("Това животно вече е осиновено и не приема нови заявки.");

            var alreadyExists = await HasUserAlreadyRequestedAsync(animalId, userId);
            if (alreadyExists)
                throw new InvalidOperationException("Вече имате подадена заявка за това животно.");

            var request = new AdoptionRequest
            {
                AnimalId = animalId,
                UserId = userId,
                FullName = fullName,
                Phone = phone,
                Email = email,
                Comment = comment,
                Status = RequestStatus.New,
                CreatedOn = DateTime.UtcNow
            };

            db.AdoptionRequests.Add(request);
            await db.SaveChangesAsync();

            return request.Id;
        }

        public async Task<List<AdoptionRequest>> GetMyRequestsAsync(string userId)
        {
            return await db.AdoptionRequests
                .AsNoTracking()
                .Include(r => r.Animal)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<AdoptionRequest>> GetAllRequestsAsync()
        {
            return await db.AdoptionRequests
                .AsNoTracking()
                .Include(r => r.Animal)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        public async Task<bool> ApproveAsync(int requestId)
        {
            // Вземаме заявката + животното
            var req = await db.AdoptionRequests
                .Include(r => r.Animal)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (req == null) return false;

            // Ако вече е обработена, не правим нищо
            if (req.Status == RequestStatus.Approved) return true;

            // Ако животното вече е осиновено, не може да одобрим нова заявка
            if (req.Animal.Status == AnimalStatus.Adopted)
                throw new InvalidOperationException("Животното вече е осиновено.");

            // 1) Одобряваме тази заявка
            req.Status = RequestStatus.Approved;

            // 2) Маркираме животното като осиновено
            req.Animal.Status = AnimalStatus.Adopted;

            // 3) Отказваме всички други заявки за същото животно
            var otherRequests = await db.AdoptionRequests
                .Where(r => r.AnimalId == req.AnimalId && r.Id != req.Id && r.Status == RequestStatus.New)
                .ToListAsync();

            foreach (var r in otherRequests)
                r.Status = RequestStatus.Rejected;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectAsync(int requestId)
        {
            var req = await db.AdoptionRequests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (req == null) return false;

            // ако вече е отказана, няма проблем
            if (req.Status == RequestStatus.Rejected) return true;

            // ако е одобрена, не позволяваме да я откажем (по-логично)
            if (req.Status == RequestStatus.Approved)
                throw new InvalidOperationException("Одобрена заявка не може да бъде отказана.");

            req.Status = RequestStatus.Rejected;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<AdoptionRequest?> GetByIdAsync(int id)
        {
            return await db.AdoptionRequests
                .Include(r => r.Animal)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> SetStatusAsync(int requestId, RequestStatus status)
        {
            var req = await db.AdoptionRequests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (req == null) return false;

            req.Status = status;
            await db.SaveChangesAsync();
            return true;
        }
    }
}
