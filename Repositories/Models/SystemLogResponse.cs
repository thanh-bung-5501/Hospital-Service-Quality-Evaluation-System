namespace Repositories.Models
{
    public class SystemLogResponse
    {
        public int LogId { get; set; }

        public string Email { get; set; }

        public string Note { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
