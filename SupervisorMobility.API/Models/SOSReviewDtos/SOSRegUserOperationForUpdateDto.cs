using SupervisorMobility.API.DataAccess.Entities.SOS_Review;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Models.SOSReviewDtos
{
    public class SOSRegUserOperationForUpdateDto
    {
        public int? SOSReviewProgramid { get; set; }

        public int? OperationId { get; set; }

        public int? SupervisorId { get; set; }
    }
}
