using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;

namespace SupervisorMobility.API.Models.HRIHourmeterRevisionDto
{
    public class GetHourmeterRevisionDto
    {
        public int Id { get; set; }
        public int? HriId { get; set; }
        public List<GetDailyRevisionDto>? DailyRevisions { get; set; }
    }
}
