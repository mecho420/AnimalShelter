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
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public CreateModel(ApplicationDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
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

            string imagePath;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                try
                {
                    imagePath = await _imageService.SaveAnimalImageAsync(ImageFile);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return Page();
                }
            }
            else
            {
                // default снимка (по желание може да е според Species)
                imagePath = "/images/animals/default-dog.jpg";
            }

            var animal = new Animal
            {
                Name = Input.Name,
                Species = Input.Species,
                Age = Input.Age,
                Gender = Input.Gender,
                Description = Input.Description,
                HealthInfo = Input.HealthInfo,
                Status = Input.Status,
                ImagePath = imagePath
            };

            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Animals/Details", new { id = animal.Id });
        }
    }
}
