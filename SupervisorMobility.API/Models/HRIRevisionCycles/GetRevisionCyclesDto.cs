

using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.Models.HRIRevisionCycles
{
    public class GetRevisionCyclesDto
    {
        public int RevisionCycleId { get; set; }
        public int Cycle { get; set; }
        public int? HRIRevisionItemsId { get; set; }
        public HRIRevisionItems? HRIRevisionItems { get; set; }
        public List<DailyRevisions>? DailyRevisions { get; set; }
    }
}
