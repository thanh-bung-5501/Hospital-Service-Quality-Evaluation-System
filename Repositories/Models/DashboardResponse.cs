namespace Repositories.Models
{
    public class DashboardResponse
    {
        public int NumberOfEvaluations { get; set; }
        public decimal PercentVsPreMonthOfNumEva { get; set; }
        public decimal PercentOfPatientEvaluated { get; set; }
        public decimal PercentVsPreMonthOfPatEva { get; set; }
        public decimal PercentOfConcurLevel { get; set; }
        public decimal PercentVsPreMonthOfConcurLevel { get; set; }

        // PieChart
        public List<OverallEvaluativeLevel> OverallEvaluativeLevels { get; set; }

        // LineChart
        public ChartMui NumberOfPatientsOverTimeForServices { get; set; }

        // BarChart
        public ChartMui SummaryOfEvaluatedServices { get; set; }
    }
}
