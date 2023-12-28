namespace Repositories.Models
{
    public class EvaluationResultForPatientResponse
    {
        public int EvaDataId { get; set; }
        public string? Option { get; set; }
        public int? Point { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
