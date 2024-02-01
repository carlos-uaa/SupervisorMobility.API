using SupervisorMobility.API.DataAccess.Entities.SOS_Review;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Models.SOSReviewDtos
{
    public class SOSReviewDistSuggestionForCreateDto
    {

        public int? SOSReviewProgramid { get; set; }

        public int DistributionId { get; set; }

        public bool SuggestionApplied { get; set; }
    }
}
