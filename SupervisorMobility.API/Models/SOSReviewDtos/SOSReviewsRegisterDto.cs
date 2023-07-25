using SupervisorMobility.API.DataAccess.Entities.SOS_Review;
using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Models.JobObservationDtos;
using SupervisorMobility.API.Models.DistributionDtos;

namespace SupervisorMobility.API.Models.SOSReviewDtos
{
    public class SOSReviewsRegisterDto
    {
        public int SOSRegisterJobid { get; set; }
        public int? JobObservationId { get; set; }
        public JobObservationDto? JobObservation { get; set; }
        public int? DistributionId { get; set; }
        public DistributionWithoutNavigationPropertiesDto? Distribution { get; set; }
        public int? SOSReviewProgramid { get; set; }
        public SOSReviewWithOutDataDto? SOSReviewProgram { get; set; }
        public DateTime? PreviewDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
