using AnimalShelter.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalShelter.Services
{
    public interface IAnimalService
    {
        Task<List<Animal>> GetPublicAnimalsAsync();
        Task<List<Animal>> GetAdminAnimalsAsync();
        Task<int> CreateAsync(Animal animal);
        Task<bool> UpdateAsync(Animal updatedAnimal, IFormFile? newImageFile);
        Task<Animal?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
