namespace Repositories.Models
{
    public class EvaluationResultForPatientRequest
    {
        public int serId { get; set; }
        public string patId { get; set; }
        public DateTime createdOn { get; set; }
    }
}
