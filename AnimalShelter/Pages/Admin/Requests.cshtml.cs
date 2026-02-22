using AnimalShelter.Models;
using AnimalShelter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnimalShelter.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class RequestsModel : PageModel
    {
        private readonly IAdoptionService adoptionService;

        public RequestsModel(IAdoptionService adoptionService)
        {
            this.adoptionService = adoptionService;
        }

        public List<AdoptionRequest> Requests { get; set; } = new();

        public async Task OnGetAsync()
        {
            Requests = await adoptionService.GetAllRequestsAsync();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            try
            {
                await adoptionService.ApproveAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            try
            {
                await adoptionService.RejectAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage();
        }
    }
}