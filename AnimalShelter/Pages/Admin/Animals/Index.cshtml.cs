using AnimalShelter.Common;
using AnimalShelter.Models;
using AnimalShelter.Models.Enums;
using AnimalShelter.Services;
using AnimalShelter.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public PagedResult<Animal> Result { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Species { get; set; }

        [BindProperty(SupportsGet = true)]
        public AnimalStatus? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public async Task OnGetAsync()
        {
            var filter = new AnimalFilterModel
            {
                SearchTerm = SearchTerm,
                Species = Species,
                Status = Status,
                PageNumber = PageNumber,
                PageSize = 10
            };

            Result = await animalService.GetAdminAnimalsAsync(filter);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await animalService.DeleteAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage();
        }
    }
}
