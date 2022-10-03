using SupervisorMobility.API.Models.AreaDtos;

namespace SupervisorMobility.API.Models.PlantDtos
{
    public class PlantWithJustAreasDto
    {
        public int PlantId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }

        public ICollection<AreaWithoutNavigationPropertiesDto> Areas { get; set; }
            = new List<AreaWithoutNavigationPropertiesDto>();
    }
}
