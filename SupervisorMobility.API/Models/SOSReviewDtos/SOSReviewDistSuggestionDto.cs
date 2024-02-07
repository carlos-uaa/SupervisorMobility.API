namespace SupervisorMobility.API.Models.SOSReviewDtos
{
    public class SOSReviewDistSuggestionDto
    {
        public int SOSReviewDistSuggestionId { get; set; }
        public int? SOSReviewProgramid { get; set; }

        public int DistributionId { get; set; }

        public bool SuggestionApplied { get; set; }
    }
}
