namespace SupervisorMobility.API.Models.GlosaryDtos
{
    public class GlosaryForUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
