using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.HRIWeeklyRevisions
{
    public class GetWeeklyRevisionDto
    {
        public int RevisionId { get; set; }
        public int HriId { get; set; }
        public int? UserId { get; set; }
        public GetUserForHRIDailyRevsionDto? SeniorSupervisor { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public bool? IsActive { get; set; }
        public string? Status { get; set; }
         
    }
}
