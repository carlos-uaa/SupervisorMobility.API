namespace SupervisorMobility.API.Models.ProductDistributionsDtos
{
    public class ProductDistributionWithoutNavigationPropertiesDto
    {
        public int ProductDistributionId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
