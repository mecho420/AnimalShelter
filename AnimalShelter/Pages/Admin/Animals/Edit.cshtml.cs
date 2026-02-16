using AnimalShelter.Data;
using AnimalShelter.Helpers;
using AnimalShelter.Models;
using AnimalShelter.Models.Enums;
using AnimalShelter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Pages.Admin.Animals
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IAnimalService animalService;

        public EditModel(IAnimalService animalService)
        {
            this.animalService = animalService;
        }

        [BindProperty]
        public Animal Animal { get; set; } = default!;

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public string CurrentImagePath { get; set; } = "/images/animals/default-dog.jpg";

        public List<SelectListItem> GenderOptions { get; set; } = new();
        public List<SelectListItem> StatusOptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            LoadDropdowns();

            var animal = await animalService.GetByIdAsync(id);
            if (animal == null)
                return NotFound();

            Animal = animal;
            CurrentImagePath = string.IsNullOrWhiteSpace(Animal.ImagePath) ? CurrentImagePath : Animal.ImagePath;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            LoadDropdowns();

            if (!ModelState.IsValid)
            {
                CurrentImagePath = string.IsNullOrWhiteSpace(Animal.ImagePath) ? CurrentImagePath : Animal.ImagePath;
                return Page();
            }

            try
            {
                var ok = await animalService.UpdateAsync(Animal, ImageFile);
                if (!ok) return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                CurrentImagePath = string.IsNullOrWhiteSpace(Animal.ImagePath) ? CurrentImagePath : Animal.ImagePath;
                return Page();
            }

            return RedirectToPage("/Admin/Animals/Index");
        }

        private void LoadDropdowns()
        {
            GenderOptions = Enum.GetValues(typeof(Gender))
                .Cast<Gender>()
                .Select(g => new SelectListItem
                {
                     Value = g.ToString(),
                     Text = g.GetDisplayName()
                })
                 .ToList();

            StatusOptions = Enum.GetValues(typeof(AnimalStatus))
                .Cast<AnimalStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.GetDisplayName()
                })
                .ToList();
        }
    }
}
