using AnimalShelter.Models.Enums;

namespace AnimalShelter.ViewModels
{
    public class AnimalFilterModel
    {
        public string? Species { get; set; }

        public Gender? Gender { get; set; }

        public AnimalStatus? Status { get; set; }

        public string? SearchTerm { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 6;
    }
}