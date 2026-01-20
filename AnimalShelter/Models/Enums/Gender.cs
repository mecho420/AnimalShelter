using System.ComponentModel.DataAnnotations;

namespace AnimalShelter.Models.Enums
{
    public enum Gender
    {
        [Display(Name = "Неизвестен")]
        Unknown = 0,

        [Display(Name = "Мъжки")]
        Male = 1,

        [Display(Name = "Женски")]
        Female = 2
    }
}
