namespace Repositories.Models
{
    public class ServiceFeedbackResponse
    {
        public long FbId { get; set; }
        public string? Feedback { get; set; }
        public string? PatientId { get; set; }
        public int? SerId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? Status { get; set; }
    }
}
