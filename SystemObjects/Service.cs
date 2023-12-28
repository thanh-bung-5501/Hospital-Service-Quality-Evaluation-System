using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemObjects
{
    public class Service
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SerId { get; set; }

        [StringLength(150)]
        public string? SerName { get; set; }

        public string? SerDesc { get; set; }

        public string? Icon { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public bool? Status { get; set; }
    }
}
