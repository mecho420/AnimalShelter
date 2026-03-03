using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AnimalShelter.Data;
using AnimalShelter.Models;
using AnimalShelter.Models.Enums;
using AnimalShelter.Services;

namespace AnimalShelter.Pages.Adoptions
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IAnimalService animalService;
        private readonly IAdoptionService adoptionService;

        public CreateModel(IAnimalService animalService, IAdoptionService adoptionService)
        {
            this.animalService = animalService;
            this.adoptionService = adoptionService;
        }

        public Animal Animal { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Display(Name = "Име и фамилия")]
            [Required(ErrorMessage = "Моля, въведете име и фамилия.")]
            [StringLength(100)]
            public string FullName { get; set; } = "";

            [Display(Name = "Телефон")]
            [Required(ErrorMessage = "Моля, въведете телефон.")]
            [StringLength(30)]
            public string Phone { get; set; } = "";

            [Display(Name = "Имейл")]
            [Required(ErrorMessage = "Моля, въведете имейл.")]
            [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
            [StringLength(100)]
            public string Email { get; set; } = "";

            [Display(Name = "Коментар (по желание)")]
            [StringLength(1000)]
            public string? Comment { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int animalId)
        {
            var animal = await animalService.GetByIdAsync(animalId);
            if (animal == null) return NotFound();

            if (animal.Status != AnimalStatus.ForAdoption)
            {
                TempData["Error"] = "Това животно вече е осиновено и не приема нови заявки.";
                return RedirectToPage("/Animals/Details", new { id = animalId });
            }

            Animal = animal;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int animalId)
        {
            var animal = await animalService.GetByIdAsync(animalId);
            if (animal == null) return NotFound();

            if (animal.Status != AnimalStatus.ForAdoption)
            {
                TempData["Error"] = "Това животно вече е осиновено и не приема нови заявки.";
                return RedirectToPage("/Animals/Details", new { id = animalId });
            }

            if (!ModelState.IsValid)
            {
                Animal = animal;
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Challenge();

            try
            {
                await adoptionService.CreateRequestAsync(
                    animalId,
                    userId,
                    Input.FullName,
                    Input.Phone,
                    Input.Email,
                    Input.Comment
                );
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Animal = animal;
                return Page();
            }

            return RedirectToPage("/Adoptions/My");
        }
    }
}
