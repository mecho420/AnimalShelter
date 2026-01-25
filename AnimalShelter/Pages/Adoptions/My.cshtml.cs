using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AnimalShelter.Data;
using AnimalShelter.Models;

namespace AnimalShelter.Pages.Adoptions
{
    [Authorize]
    public class MyModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MyModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<AdoptionRequest> Requests { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Requests = await _context.AdoptionRequests
                .Include(r => r.Animal)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }
    }
}
