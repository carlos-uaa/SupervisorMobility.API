namespace SupervisorMobility.API.Models.DistributionDtos
{
    public class DistributionWithoutNavigationPropertiesDto
    {
        public int DistributionId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public int CriticalType { get; set; }

    }
}
