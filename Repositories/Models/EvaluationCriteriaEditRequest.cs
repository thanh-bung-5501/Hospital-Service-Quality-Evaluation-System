namespace Repositories.Models
{
    public class EvaluationCriteriaEditRequest
    {
        public int CriId { get; set; }
        public string? CriDesc { get; set; } = string.Empty;
        //public int ModifiedBy { get; set; }
        public string? Note { get; set; } = string.Empty;
        public int SerId { get; set; }
    }
}
