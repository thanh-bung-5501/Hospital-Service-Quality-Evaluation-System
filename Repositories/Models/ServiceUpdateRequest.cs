using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class ServiceUpdateRequest
    {
        [Required]
        public int SerId { get; set; }

        [Required]
        [StringLength(150)]
        public string SerName { get; set; }

        [Required]
        public string SerDesc { get; set; }

        public IFormFile? Icon { get; set; }

    }
}
