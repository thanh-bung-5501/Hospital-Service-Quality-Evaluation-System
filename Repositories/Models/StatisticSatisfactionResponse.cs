namespace Repositories.Models
{
    public class StatisticSatisfactionResponse
    {
        public List<string> Labels { get; set; }
        public List<SatisfactionDegreeByService> Data { get; set; }
    }
}
