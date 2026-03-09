using AnimalShelter.Common;
using AnimalShelter.Models;
using AnimalShelter.Models.Enums;
using AnimalShelter.Services;
using AnimalShelter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimalShelter.Pages.Animals
{
    public class IndexModel : PageModel
    {
        private readonly IAnimalService animalService;

        public IndexModel(IAnimalService animalService)
        {
            this.animalService = animalService;
        }

        public PagedResult<Animal> Result { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Species { get; set; }

        [BindProperty(SupportsGet = true)]
        public Gender? Gender { get; set; }

        [BindProperty(SupportsGet = true)]
        public AnimalStatus? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int? MinAge { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxAge { get; set; }

        public async Task OnGetAsync()
        {
            var filter = new AnimalFilterModel
            {
                Species = Species,
                Gender = Gender,
                Status = Status,
                SearchTerm = SearchTerm,
                MinAge = MinAge,
                MaxAge = MaxAge,
                PageNumber = PageNumber,
                PageSize = 6
            };

            Result = await animalService.GetFilteredAnimalsAsync(filter);
        }
    }
}