namespace Repositories.Models
{
    public class SatisfactionDegreeByService
    {
        public string ServiceName { get; set; }
        public List<decimal> PercentOfDegree { get; set; }
    }
}
