using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS_Review
{
    public class SOSReviewDistSuggestion
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSReviewDistSuggestionId { get; set; }

        public int? SOSReviewProgramid { get; set; }
        public SOSReviewProgram? SOSReviewProgram { get; set; }

        public int DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public bool SuggestionApplied { get; set; }
    }
}
