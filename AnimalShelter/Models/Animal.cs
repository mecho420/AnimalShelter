using System.ComponentModel.DataAnnotations;
using AnimalShelter.Models.Enums;

namespace AnimalShelter.Models
{
    public class Animal
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = "";

        [Required, StringLength(30)]
        public string Species { get; set; } = ""; // Dog/Cat/Other

        [Range(0, 30)]
        public int Age { get; set; }

        public Gender Gender { get; set; }

        [Required, StringLength(1000)]
        public string Description { get; set; } = "";

        [StringLength(1000)]
        public string? HealthInfo { get; set; }

        [StringLength(255)]
        public string? ImagePath { get; set; }

        public AnimalStatus Status { get; set; } = AnimalStatus.ForAdoption;
    }
}
