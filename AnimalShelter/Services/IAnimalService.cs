using AnimalShelter.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalShelter.Services
{
    public interface IAnimalService
    {
        Task<List<Animal>> GetPublicAnimalsAsync();
        Task<Animal?> GetByIdAsync(int id);

        Task<List<Animal>> GetAdminAnimalsAsync();
        Task<int> CreateAsync(Animal animal);
        Task<bool> UpdateAsync(Animal animal);
        Task<bool> DeleteAsync(int id);
    }
}
