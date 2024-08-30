using SupervisorMobility.API.DataAccess.Entities.SOS;

namespace SupervisorMobility.API.Models.SOS.ToolsUsedDtos
{
    public class ToolUsedForUpdateDto
    {
        public int ToolUsedId { get; set; }

        public int ToolId { get; set; }

        public double Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}
