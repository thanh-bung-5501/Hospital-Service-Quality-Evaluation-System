namespace Repositories.Models
{
    public class EvaluationCriteriaResponse
    {
        public int CriId { get; set; }
        public string? CriDesc { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string? Note { get; set; }
        public int SerId { get; set; }
        public bool? Status { get; set; }
        public List<UserResponse>? Users { get; set; }
        public ServiceResponse? Service { get; set; }
    }
}
