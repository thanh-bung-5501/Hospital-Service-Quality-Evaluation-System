using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceEvaObjects
{
    public class ServiceFeedback
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FbId { get; set; }

        public string? Feedback { get; set; }

        [StringLength(8)]
        public string? PatientId { get; set; }

        public int? SerId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public bool? Status { get; set; }
    }
}
