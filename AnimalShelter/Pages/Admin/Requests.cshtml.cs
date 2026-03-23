using AnimalShelter.Models;
using AnimalShelter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AnimalShelter.Common;
using AnimalShelter.Models.Enums;

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

        public PagedResult<AdoptionRequest> Result { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? AnimalName { get; set; }

        [BindProperty(SupportsGet = true)]
        public RequestStatus? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public async Task OnGetAsync()
        {
            Result = await adoptionService.GetFilteredRequestsAsync(
                SearchTerm,
                AnimalName,
                Status,
                PageNumber,
                5);
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

            return RedirectToPage(new
            {
                SearchTerm,
                AnimalName,
                Status,
                PageNumber
            });
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

            return RedirectToPage(new
            {
                SearchTerm,
                AnimalName,
                Status,
                PageNumber
            });
        }
    }
}