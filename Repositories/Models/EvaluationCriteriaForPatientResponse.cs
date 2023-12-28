namespace Repositories.Models
{
    public class EvaluationCriteriaForPatientResponse
    {
        public int CriId { get; set; }
        public string? CriDesc { get; set; }
        public string? Note { get; set; }
        public EvaluationResultForPatientResponse? ResultForPatient { get; set; }
    }
}
