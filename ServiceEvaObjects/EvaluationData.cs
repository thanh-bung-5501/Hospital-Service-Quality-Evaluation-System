using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServiceEvaObjects
{
    public class EvaluationData
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EvaDataId { get; set; }

        public int? Point { get; set; }

        public DateTime? CreatedOn { get; set; }

        [StringLength(8)]
        public string? PatientId { get; set; }

        public int? CriId { get; set; }

        public bool? Status { get; set; }
    }
}
