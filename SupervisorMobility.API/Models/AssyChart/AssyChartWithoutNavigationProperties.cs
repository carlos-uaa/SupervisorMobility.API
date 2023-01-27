namespace SupervisorMobility.API.Models.AssyChart
{
    public class AssyChartWithoutNavigationProperties
    {
        public int AssyChardId { get; set; }
        public bool? IsActive { get; set; }
        public string GOS { get; set; } = string.Empty;
        public string CCP { get; set; } = string.Empty;
        public string HOE { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public int idProduct { get; set; }
        //Linkers o Navigation Propietis
        public int PlantId { get; set; }
        public int AreaId { get; set; }
        public int DistributionId { get; set; }
    }
}
