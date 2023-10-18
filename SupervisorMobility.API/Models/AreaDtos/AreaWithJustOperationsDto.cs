using SupervisorMobility.API.Models.DistributionDtos;

namespace SupervisorMobility.API.Models.AreaDtos
{
    public class AreaWithJustOperationsDto
    {
        public int AreaId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }

        public ICollection<DistributionWithoutNavigationPropertiesDto> Operations { get; set; }
            = new List<DistributionWithoutNavigationPropertiesDto>();
    }
}
