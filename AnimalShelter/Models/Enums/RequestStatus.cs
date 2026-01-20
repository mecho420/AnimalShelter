using System.ComponentModel.DataAnnotations;

namespace AnimalShelter.Models.Enums
{
    public enum RequestStatus 
    {
        [Display(Name = "Нов")]
        New = 0,

        [Display(Name = "Удобрено")]
        Approved = 1,

        [Display(Name = "Отказано")]
        Rejected = 2 
    }
}
