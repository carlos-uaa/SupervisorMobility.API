using SupervisorMobility.API.Models.JobObservationDtos;

namespace SupervisorMobility.API.Models.SOSReviewDtos
{
    public class SOSReviewsRegisterForUpdateDto
    {
        public int SOSRegisterJobid { get; set; }
        public int? SOSReviewProgramid { get; set; }
        public int? JobObservationId { get; set; }
        public int? OperationId { get; set; }
        public string? Commentary { get; set; }
        public DateTime? PreviewDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
