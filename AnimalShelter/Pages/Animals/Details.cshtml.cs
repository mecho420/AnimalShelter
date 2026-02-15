using AnimalShelter.Models;
using AnimalShelter.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace AnimalShelter.Pages.Animals
{
    public class DetailsModel : PageModel
    {
        private readonly IAnimalService animalService;

        public DetailsModel(IAnimalService animalService)
        {
            this.animalService = animalService;
        }

        public Animal? Animal { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Animal = await animalService.GetByIdAsync(id);

            if (Animal == null)
                return NotFound();

            return Page();
        }
    }
}
