using SupervisorMobility.API.DataAccess.Entities.HRI;

namespace SupervisorMobility.API.Models.HRICyclesDtos
{
    public class GetHRICyclesDto
    {
        public int HriId { get; set; }
        public HRI HRI { get; set; }
        public int Cycle { get; set; }
        public List<DailyRevisions> DailyRevisions { get; set; }
    }
}
