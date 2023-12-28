namespace Repositories.Models
{
    public class ReportServiceResponse
    {
        public int SerId { get; set; }

        public string? SerName { get; set; }

        public string? SerDesc { get; set; }

        public string? Icon { get; set; }

        public int NumberOfEvaluated { get; set; }
    }
}
