namespace SupervisorMobility.API.Models.PillarDtos
{
    public class PillarDto
    {
        public int PillarId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
