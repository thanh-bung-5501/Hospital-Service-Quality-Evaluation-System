namespace Repositories.Models
{
    public class EvaluationCriteriaAddRequest
    {
        public string? CriDesc { get; set; } = string.Empty;
        //public int CreatedBy { get; set; }
        public string? Note { get; set; } = string.Empty;
        public int SerId { get; set; }
        public bool? Status { get; set; }
    }
}
