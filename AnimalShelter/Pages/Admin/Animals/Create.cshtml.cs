using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AnimalShelter.Data;
using AnimalShelter.Models;
using AnimalShelter.Models.Enums;
using AnimalShelter.Services;

namespace AnimalShelter.Pages.Admin.Animals
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IAnimalService animalService;

        public CreateModel(IAnimalService animalService)
        {
            this.animalService = animalService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public class InputModel
        {
            [Display(Name = "Име")]
            [Required]
            [StringLength(50)]
            public string Name { get; set; } = "";

            [Display(Name = "Вид")]
            [Required]
            [StringLength(30)]
            public string Species { get; set; } = "";

            [Display(Name = "Възраст")]
            [Range(0, 30)]
            public int Age { get; set; }

            [Display(Name = "Пол")]
            public Gender Gender { get; set; }

            [Display(Name = "Описание")]
            [Required]
            [StringLength(1000)]
            public string Description { get; set; } = "";

            [Display(Name = "Здравна информация")]
            [StringLength(1000)]
            public string? HealthInfo { get; set; }

            [Display(Name = "Статус")]
            public AnimalStatus Status { get; set; } = AnimalStatus.ForAdoption;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var animal = new Animal
            {
                Name = Input.Name,
                Species = Input.Species,
                Age = Input.Age,
                Gender = Input.Gender,
                Description = Input.Description,
                HealthInfo = Input.HealthInfo,
                Status = Input.Status
                // ImagePath ще се зададе в service
            };

            try
            {
                var id = await animalService.CreateAsync(animal, ImageFile);
                return RedirectToPage("/Animals/Details", new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
