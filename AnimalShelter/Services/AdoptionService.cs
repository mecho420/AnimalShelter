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
