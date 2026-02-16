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
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IImageService imageService;

        public EditModel(ApplicationDbContext context, IWebHostEnvironment env, IImageService imageService)
        {
            _context = context;
            _env = env;
            this.imageService = imageService;
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
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null) return NotFound();

            Animal = animal;
            CurrentImagePath = string.IsNullOrWhiteSpace(animal.ImagePath) ? CurrentImagePath : animal.ImagePath;

            LoadDropdowns();
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

            var dbAnimal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == Animal.Id);
            if (dbAnimal == null) return NotFound();

            // Update полетата
            dbAnimal.Name = Animal.Name;
            dbAnimal.Species = Animal.Species;
            dbAnimal.Age = Animal.Age;
            dbAnimal.Gender = Animal.Gender;
            dbAnimal.Status = Animal.Status;
            dbAnimal.Description = Animal.Description;
            dbAnimal.HealthInfo = Animal.HealthInfo;

            // Ако има нова снимка се качва и сменяме ImagePath
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var oldImagePath = dbAnimal.ImagePath;

                string newPath;
                try
                {
                    newPath = await imageService.SaveAnimalImageAsync(ImageFile);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    CurrentImagePath = string.IsNullOrWhiteSpace(dbAnimal.ImagePath) ? CurrentImagePath : dbAnimal.ImagePath;
                    return Page();
                }

                dbAnimal.ImagePath = newPath;

                // Трием старата снимка, само ако е custom
                imageService.DeleteAnimalImageIfCustom(oldImagePath);
            }

            await _context.SaveChangesAsync();
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
