using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.ToolDtos;

namespace SupervisorMobility.API.Models.SOS.ToolsUsedDtos
{
    public class ToolUsedDto
    {
        public int ToolUsedId { get; set; }

        public int ToolId { get; set; }
        public ToolDto Tool { get; set; }

        public double Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}
