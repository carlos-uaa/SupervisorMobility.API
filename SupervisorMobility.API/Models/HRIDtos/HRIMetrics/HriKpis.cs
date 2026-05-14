namespace SupervisorMobility.API.Models.HRIDtos.HRIMetrics
{
    public class HriKpis
    {
        public int TotalHri { get; set; }
        public int TodayRevisions { get; set; }
        public int CriticCycle { get; set; }
        public double GlobalHealth { get; set; }
    }
}
