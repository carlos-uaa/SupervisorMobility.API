using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.Users;


namespace SupervisorMobility.API.Models.HRICyclesDtos
{
    public class GetHRICyclesDto
    {
        public int CycleId { get; set; }
        public int HriId { get; set; }
        public int Cycle { get; set; }
        public bool? IsActive { get; set; }
        public int? SupervisorUserId { get; set; }
        public GetUserForHRIDailyRevsionDto? Supervisor { get; set; }
        public int? OperatorUserId { get; set; }
        public GetUserForHRIDailyRevsionDto? Operator { get; set; } = null;
        public string? UserType { get; set; }
        public List<GetDailyRevisionDto>? DailyRevisions { get; set; }
    }
}
