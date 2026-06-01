namespace SupervisorMobility.API.Models.HRIDtos.HRIMetrics
{
    public class LinesChartData
    {
        public string[] Labels { get; set; }
        public List<ChartSeries> Series { get; set; }
    }

    public class ChartSeries
    {
        public string Name { get; set; }
        public double[] Data { get; set; }
        public bool IsVisible { get; set; }
        public int Index { get; set; }
    }
}
