using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AnimalShelter.Data;
using AnimalShelter.Models;

namespace AnimalShelter.Pages.Admin.Animals
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public IndexModel(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public List<Animal> Animals { get; set; } = new();

        public async Task OnGetAsync()
        {
            Animals = await _context.Animals
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null)
                return RedirectToPage();

            // (по желание) изтриваме снимката от wwwroot, ако не е default
            if (!string.IsNullOrWhiteSpace(animal.ImagePath) &&
                animal.ImagePath.StartsWith("/images/animals/") &&
                !animal.ImagePath.EndsWith("default-dog.jpg") &&
                !animal.ImagePath.EndsWith("default-cat.jpg"))
            {
                var relative = animal.ImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                var fullPath = Path.Combine(_env.WebRootPath, relative);

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }

            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
