namespace Repositories.Models
{
    public class ServiceResponse
    {
        public int SerId { get; set; }

        public string? SerName { get; set; }

        public string? SerDesc { get; set; }

        public string? Icon { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public bool? Status { get; set; }
    }
}
