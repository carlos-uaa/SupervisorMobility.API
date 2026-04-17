using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.HRIDailyRevisionDtos
{
    public class GetDailyRevisionDto
    {
        public int RevisionId { get; set; }
        public int? RevisionCycleId { get; set; }
        public int? CycleId { get; set; }
        public int? HourmeterId { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int? UserId { get; set; }
        public GetUserForHRIDailyRevsionDto? Responsible { get; set; }
        public string? UserType { get; set; }
        public string? Status { get; set; }
    }
}
