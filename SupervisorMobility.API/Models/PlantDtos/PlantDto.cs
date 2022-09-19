namespace SupervisorMobility.API.Models.PlantDtos
{
    public class PlantDto
    {
        public int PlantId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
