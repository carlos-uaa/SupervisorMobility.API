namespace SupervisorMobility.API.Models.AreaDtos
{
    public class AreaWithoutNavigationPropertiesDto
    {
        public int AreaId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
