namespace Repositories.Models
{
    public class EvaluationCriteriaRequest
    {
        public int? serId { get; set; } = 0;
        public DateTime? from { get; set; } = DateTime.MinValue;
        public DateTime? to { get; set; } = DateTime.MaxValue;
    }
}
