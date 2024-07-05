namespace SupervisorMobility.API.Models.SOS.ToolDtos
{
    public class ToolDto
    {
        public int ToolId { get; set; }
        public string ToolCode { get; set; }
        public string ToolName { get; set; }
        public bool? IsActive { get; set; }
    }
}
