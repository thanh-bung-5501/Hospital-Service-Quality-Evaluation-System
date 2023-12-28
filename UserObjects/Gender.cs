using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserObjects
{
    public class Gender
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GenderId { get; set; }

        [StringLength(50)]
        public string GenderName { get; set; }
    }
}
