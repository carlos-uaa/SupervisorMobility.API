using SupervisorMobility.API.DataAccess.Entities.HRI;

namespace SupervisorMobility.API.Models.HRIRevisionCycles
{
    public class UpdateRevisionCycleDto
    {
        public int Cycle { get; set; }
        public int? HRIRevisionItemsId { get; set; }
    }
}
