using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class UserAddRequest
    {
        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30)]
        public string LastName { get; set; }

        [Required]
        public int GenderId { get; set; }

        public DateTime? Dob { get; set; }

        [Required]
        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        public IFormFile? Image { get; set; }

        public RoleDistributionAddRequest Role { get; set; }

        public bool Status { get; set; }
    }
}
