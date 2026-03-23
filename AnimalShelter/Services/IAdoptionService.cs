using AnimalShelter.Models;
using AnimalShelter.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnimalShelter.Common;
using AnimalShelter.Models;
using AnimalShelter.Models.Enums;

namespace AnimalShelter.Services
{
    public interface IAdoptionService
    {
        Task<bool> HasUserAlreadyRequestedAsync(int animalId, string userId);
        Task<int> CreateRequestAsync(int animalId, string userId, string fullName, string phone, string email, string? comment);
        Task<List<AdoptionRequest>> GetMyRequestsAsync(string userId);
        Task<List<AdoptionRequest>> GetAllRequestsAsync(); // admin
        Task<bool> ApproveAsync(int requestId);
        Task<bool> RejectAsync(int requestId);
        Task<AdoptionRequest?> GetByIdAsync(int id);
        Task<bool> SetStatusAsync(int requestId, RequestStatus status);

        // за страницата Create (зареждане на животното + проверки)
        Task<Animal?> GetAnimalForAdoptionRequestAsync(int animalId);

        Task<PagedResult<AdoptionRequest>> GetFilteredRequestsAsync(string? searchTerm, string? animalName, RequestStatus? status, int pageNumber, int pageSize);
    }
}
