namespace Repositories.Models
{
    public class ServiceComparisonResponse
    {
        public string Service1Name { get; set; }
        public string Service2Name { get; set; }
        public List<TypeComparison> TypeComparisons { get; set; }
    }
}
