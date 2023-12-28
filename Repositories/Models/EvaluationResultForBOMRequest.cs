namespace Repositories.Models
{
    public class EvaluationResultForBOMRequest
    {
        public int serId { get; set; } = 0;
        public string? patId { get; set; } = string.Empty;
        public DateTime? from { get; set; } = DateTime.MinValue;
        public DateTime? to { get; set; } = DateTime.MaxValue;
    }
}
