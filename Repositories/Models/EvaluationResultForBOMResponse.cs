namespace Repositories.Models
{
    public class EvaluationResultForBOMResponse
    {
        public int EvaDataId { get; set; }
        public int? Point { get; set; }
        public string? Option { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? PatientId { get; set; }
        public string? PatientName { get; set; }
        public int? CriId { get; set; }
        public string? CriDesc { get; set; }
        public int? SerId { get; set; }
        public string? SerName { get; set; }
        public long? SerFbId { get; set; }
        public string? SerFb { get; set; }
        public bool? Status { get; set; }
    }
}
