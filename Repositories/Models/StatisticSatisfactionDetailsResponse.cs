namespace Repositories.Models
{
    public class StatisticSatisfactionDetailsResponse
    {
        public string Label { get; set; }
        public List<SatisfactionDegreeByServiceDetails> Data { get; set; }
    }
}
