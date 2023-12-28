using System.ComponentModel.DataAnnotations;

namespace ServiceEvaObjects
{
    public class Patient
    {
        [Key, StringLength(8)]
        public string PatientId { get; set; }

        [StringLength(30)]
        public string? FirstName { get; set; }

        [StringLength(30)]
        public string? LastName { get; set; }

        [StringLength(30)]
        public string? PhoneNumber { get; set; }
    }
}
