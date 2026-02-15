using AnimalShelter.Models;
using AnimalShelter.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalShelter.Services
{
    public interface IAdoptionService
    {
        Task<int> CreateRequestAsync(AdoptionRequest request);
        Task<List<AdoptionRequest>> GetMyRequestsAsync(string userId);
        Task<List<AdoptionRequest>> GetAllRequestsAsync(); // admin
        Task<AdoptionRequest?> GetByIdAsync(int id);
        Task<bool> SetStatusAsync(int requestId, RequestStatus status);
    }
}
