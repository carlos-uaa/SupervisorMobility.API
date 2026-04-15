using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.Models.HRIHourmeterRevisionDto
{
    public class GetHourmeterRevisionDto
    {
        public int Id { get; set; }
        public int? HriId { get; set; }
        public List<DailyRevisions>? DailyRevisions { get; set; }
    }
}
