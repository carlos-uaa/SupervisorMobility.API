namespace SupervisorMobility.API.Models.MetricsDtos
{
    public class MetricsFiltersDto
    {
        public int? plantId { get; set; }
        public int? areaId { get; set; }
        public int? distributionId { get; set; }
        public int? operationId { get; set; }
        public DateTime? inferiorDate { get; set; }
        public DateTime? superiorDate { get; set; }
        public DateTime? today { get; set; }
    }
}
