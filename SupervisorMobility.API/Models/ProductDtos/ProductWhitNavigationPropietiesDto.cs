using SupervisorMobility.API.Models.DistributionDtos;

namespace SupervisorMobility.API.Models.ProductDtos
{
    public class ProductWhitNavigationPropietiesDto
    {
        public int ProductId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }

        public ICollection<DistributionWithoutNavigationPropertiesDto>? Distributions { get; set; } = new List<DistributionWithoutNavigationPropertiesDto>();
    }
}
