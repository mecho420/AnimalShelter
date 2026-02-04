using Microsoft.AspNetCore.Http;

namespace AnimalShelter.Services
{
    public interface IImageService
    {
        Task<string> SaveAnimalImageAsync(IFormFile file);
    }
}
