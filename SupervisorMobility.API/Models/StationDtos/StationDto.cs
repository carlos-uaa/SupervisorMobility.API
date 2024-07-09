namespace SupervisorMobility.API.Models.StationDtos
{
    public class StationDto
    {
        public int StationId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
