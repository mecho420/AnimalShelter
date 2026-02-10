using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AnimalShelter.Data;
using AnimalShelter.Models;
using AnimalShelter.Models.Enums;

namespace AnimalShelter.Pages.Adoptions
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
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
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal == null) return NotFound();

            // Не позволявай заявка за осиновено животно
            if (animal.Status == AnimalStatus.Adopted)
                return RedirectToPage("/Animals/Details", new { id = animalId });

            Animal = animal;

            if (Animal.Status != AnimalStatus.ForAdoption)
            {
                TempData["Error"] = "Това животно вече е осиновено и не приема нови заявки.";
                return RedirectToPage("/Animals/Details", new { id = Animal.Id });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int animalId)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal == null) return NotFound();

            if (animal.Status != AnimalStatus.ForAdoption)
            {
                TempData["Error"] = "Това животно вече е осиновено и не приема нови заявки.";
                return RedirectToPage("/Animals/Details", new { id = animal.Id });
            }

            if (!ModelState.IsValid)
            {
                Animal = animal;
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Challenge();

            // (по желание) блокирай дублирана заявка от същия потребител за същото животно
            var alreadyExists = await _context.AdoptionRequests.AnyAsync(r =>
                r.AnimalId == animalId && r.UserId == userId);

            if (alreadyExists)
            {
                ModelState.AddModelError(string.Empty, "Вече имате подадена заявка за това животно.");
                Animal = animal;
                return Page();
            }

            var request = new AdoptionRequest
            {
                AnimalId = animalId,
                UserId = userId,
                FullName = Input.FullName,
                Phone = Input.Phone,
                Email = Input.Email,
                Comment = Input.Comment,
                Status = RequestStatus.New,
                CreatedOn = DateTime.UtcNow
            };

            _context.AdoptionRequests.Add(request);
            await _context.SaveChangesAsync();

            // Пренасочване към "Моите заявки" (ще я направя след малко) 
            return RedirectToPage("/Adoptions/My");
        }
    }
}
