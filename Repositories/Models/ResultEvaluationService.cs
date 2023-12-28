namespace Repositories.Models
{
    public class ResultEvaluationService
    {
        public int SerId { get; set; }

        public string? SerName { get; set; }

        public string? SerDesc { get; set; }

        public string? Icon { get; set; }

        public List<ResultEvaluationCriteria> Criterias { get; set; }
    }
}
