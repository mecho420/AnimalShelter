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

        public async Task<int> CreateRequestAsync(AdoptionRequest request)
        {
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
