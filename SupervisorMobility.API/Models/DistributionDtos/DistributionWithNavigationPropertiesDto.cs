using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.OperationDtos;
using System.Collections.ObjectModel;

namespace SupervisorMobility.API.Models.DistributionDtos
{
    public class DistributionWithNavigationPropertiesDto
    {
        public int DistributionId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }

        public ICollection<ProductDtos.ProductDto> Products { get; set; } 
            = new List<ProductDtos.ProductDto>();
        public ICollection<OperationWithoutNavigationPropertiesDto> Operations { get; set; }
            = new List<OperationWithoutNavigationPropertiesDto>();
    }
}
