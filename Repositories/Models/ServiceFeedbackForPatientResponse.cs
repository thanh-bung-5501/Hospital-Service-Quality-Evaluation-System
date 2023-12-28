namespace Repositories.Models
{
    public class ServiceFeedbackForPatientResponse
    {
        public long FbId { get; set; }

        public string? Feedback { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
