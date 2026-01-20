using System.ComponentModel.DataAnnotations;

namespace AnimalShelter.Models.Enums
{
    public enum AnimalStatus
    {
        [Display(Name = "За осиновяване")]
        ForAdoption = 0,

        [Display(Name = "Осиновено")]
        Adopted = 1
    }
}
