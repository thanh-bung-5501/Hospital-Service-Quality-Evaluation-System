using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class UserChangePasswordRequest
    {
        [Required]
        [StringLength(255)]
        public string OldPassword { get; set; }
        [Required]
        [StringLength(255)]
        public string NewPassword { get; set; }
    }
}