using System.ComponentModel.DataAnnotations;
using AnimalShelter.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace AnimalShelter.Models
{
    public class AdoptionRequest
    {
        public int Id { get; set; }

        [Required]
        public int AnimalId { get; set; }
        public Animal? Animal { get; set; }

        [Required]
        public string UserId { get; set; } = "";
        public IdentityUser? User { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = "";

        [Required, StringLength(30)]
        public string Phone { get; set; } = "";

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = "";

        [StringLength(1000)]
        public string? Comment { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.New;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
