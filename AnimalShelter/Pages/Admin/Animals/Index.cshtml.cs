using AnimalShelter.Models;
using AnimalShelter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalShelter.Pages.Admin.Animals
{
    [Authorize(Roles = "Admin")]
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
            Animals = await animalService.GetAdminAnimalsAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await animalService.DeleteAsync(id);
            return RedirectToPage();
        }
    }
}
