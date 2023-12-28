using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class ServiceCreateRequest
    {
        [Required]
        [StringLength(150)]
        public string? SerName { get; set; }

        [Required]
        public string? SerDesc { get; set; }

        [Required]
        public IFormFile Icon { get; set; }

        [Required]
        public bool? Status { get; set; }
    }
}
