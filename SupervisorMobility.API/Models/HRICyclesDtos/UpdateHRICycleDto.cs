

using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.Models.HRICyclesDtos
{
    public class UpdateHRICycleDto
    {
        public HRI HRI { get; set; }
        public int Cycle { get; set; }
        public List<DailyRevisions> DailyRevisions { get; set; }
    }
}
