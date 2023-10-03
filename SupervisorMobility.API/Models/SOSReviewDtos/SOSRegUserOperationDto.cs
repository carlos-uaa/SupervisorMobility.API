using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOSReviewDtos
{
    public class SOSRegUserOperationDto
    {
        public int SOSRegUserOperationId { get; set; }

        public int? SOSReviewProgramid { get; set; }
        public SOSReviewWithOutDataDto? SOSReviewProgram { get; set; }

        public int? OperationId { get; set; }
        public OperationWithoutNavigationPropertiesDto? Operation { get; set; }

        public int? SupervisorId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails? Supervisor { get; set; }
    }
}
