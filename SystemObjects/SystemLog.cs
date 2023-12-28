using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SystemObjects
{
    public class SystemLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }
        
        public int UserId { get; set; }

        public string Note { get; set; }

        public DateTime? CreatedOn { get; set; }

    }
}
