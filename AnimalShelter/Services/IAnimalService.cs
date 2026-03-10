using AnimalShelter.Common;
using AnimalShelter.Models;
using AnimalShelter.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AnimalShelter.Services
{
    public interface IAnimalService
    {
        Task<List<Animal>> GetPublicAnimalsAsync();
        Task<List<Animal>> GetAdminAnimalsAsync();
        Task<int> CreateAsync(Animal animal, IFormFile? imageFile);
        Task<bool> UpdateAsync(Animal updatedAnimal, IFormFile? newImageFile);
        Task<Animal?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<Animal>> GetFilteredAnimalsAsync(AnimalFilterModel filter);
        Task<PagedResult<Animal>> GetAdminAnimalsAsync(AnimalFilterModel filter);
    }
}
