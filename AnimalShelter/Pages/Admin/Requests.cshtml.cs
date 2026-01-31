using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AnimalShelter.Data;
using AnimalShelter.Models.Enums;

namespace AnimalShelter.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class RequestsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RequestsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<AnimalShelter.Models.AdoptionRequest> Requests { get; set; } = new();

        public async Task OnGetAsync()
        {
            Requests = await _context.AdoptionRequests
                .Include(r => r.Animal)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var req = await _context.AdoptionRequests
                .Include(r => r.Animal)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (req == null) return NotFound();

            req.Status = RequestStatus.Approved;

            // Маркирай животното като осиновено
            if (req.Animal != null)
                req.Animal.Status = AnimalStatus.Adopted;

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            var req = await _context.AdoptionRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (req == null) return NotFound();

            req.Status = RequestStatus.Rejected;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
