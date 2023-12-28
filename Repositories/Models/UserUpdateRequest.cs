using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class UserUpdateRequest
    {
        public int UserId { get; set; }

        [StringLength(255)]
        public string? Password { get; set; }

        [StringLength(30)]
        public string? FirstName { get; set; }

        [StringLength(30)]
        public string? LastName { get; set; }

        public int? GenderId { get; set; }

        public DateTime? Dob { get; set; }

        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        public IFormFile? Image { get; set; }

        public RoleDistributionAddRequest? Role { get; set; }

        public bool? Status { get; set; }
    }
}
