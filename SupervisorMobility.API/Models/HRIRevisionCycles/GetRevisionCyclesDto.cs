

using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;

namespace SupervisorMobility.API.Models.HRIRevisionCycles
{
    public class GetRevisionCyclesDto
    {
        public int RevisionCycleId { get; set; }
        public int Cycle { get; set; }
        public int? HRIRevisionItemsId { get; set; }
        public List<GetDailyRevisionDto>? DailyRevisions { get; set; }
    }
}
