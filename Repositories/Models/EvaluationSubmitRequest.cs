namespace Repositories.Models
{
    public class EvaluationSubmitRequest
    {
        public int SerId { get; set; }

        public string? Feedback { get; set; }

        public List<EvaluationDataAnswer> EvaluationData { get; set; }
    }
}
