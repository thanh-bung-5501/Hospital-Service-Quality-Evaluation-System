using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemObjects
{
    public class SystemInformation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SysId { get; set; }
        [StringLength(255)]
        public string? SysName { get; set; }

        [StringLength(255)]
        public string? Icon { get; set; }

        [StringLength(255)]
        public string? Logo { get; set; }

        public string? Zalo { get; set; }

        [StringLength(30)]
        public string? Hotline { get; set; }

        public string? Address { get; set; }

    }
}
