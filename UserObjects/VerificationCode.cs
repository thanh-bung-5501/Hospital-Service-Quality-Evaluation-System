using System.ComponentModel.DataAnnotations;


namespace UserObjects
{
    public class VerificationCode
    {
        [Key, StringLength(255)]
        public string Email { get; set; }

        [StringLength(6)]
        public string? Code { get; set; }

        public DateTime? CodeExpires { get; set; }
    }
}
