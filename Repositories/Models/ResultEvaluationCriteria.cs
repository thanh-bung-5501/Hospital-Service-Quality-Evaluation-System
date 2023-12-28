namespace Repositories.Models
{
    public class ResultEvaluationCriteria
    {
        public int CriId { get; set; }
        public string? CriDesc { get; set; }
        public int Vote { get; set; }
        public List<ResultEvaluationOption> Options { get; set; }   
    }
}
