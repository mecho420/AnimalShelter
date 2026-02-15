using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AnimalShelter.Data;
using AnimalShelter.Models;
using AnimalShelter.Models.Enums;
using AnimalShelter.Services;

namespace AnimalShelter.Pages.Animals
{
    public class IndexModel : PageModel
    {
        private readonly IAnimalService animalService;

        public IndexModel(IAnimalService animalService)
        {
            this.animalService = animalService;
        }


        public List<Animal> Animals { get; set; } = new();

        public async Task OnGetAsync()
        {
            Animals = await animalService.GetPublicAnimalsAsync();
        }
    }
}