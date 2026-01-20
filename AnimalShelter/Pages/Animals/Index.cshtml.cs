using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AnimalShelter.Data;
using AnimalShelter.Models;
using AnimalShelter.Models.Enums;

namespace AnimalShelter.Pages.Animals
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Animal> Animals { get; set; } = new();

        public async Task OnGetAsync()
        {
            Animals = await _context.Animals
                .Where(a => a.Status == AnimalStatus.ForAdoption)
                .ToListAsync();
        }
    }
}