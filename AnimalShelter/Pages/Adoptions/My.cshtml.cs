using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AnimalShelter.Models;
using AnimalShelter.Services;

namespace AnimalShelter.Pages.Adoptions
{
    [Authorize]
    public class MyModel : PageModel
    {
        private readonly IAdoptionService adoptionService;

        public MyModel(IAdoptionService adoptionService)
        {
            this.adoptionService = adoptionService;
        }

        public List<AdoptionRequest> Requests { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                Requests = new();
                return;
            }

            Requests = await adoptionService.GetMyRequestsAsync(userId);
        }
    }
}