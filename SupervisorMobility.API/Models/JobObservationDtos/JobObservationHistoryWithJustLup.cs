using SupervisorMobility.API.Models.LupDtos;

namespace SupervisorMobility.API.Models.JobObservationDtos
{
    public class JobObservationHistoryWithJustLup
    {
        public ICollection<LupDto> Lup { get; set; } = new List<LupDto>();

        public int JobObservationId { get; set; }
        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? DistributionId { get; set; }
        public int? OperationId { get; set; }
        public int? SupervisorId { get; set; }
        public int? OperatorId { get; set; }
        public int? Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? FinishedDate { get; set; }

        public string? Justification { get; set; }

        public int? Status { get; set; }

        public int? Option { get; set; }
        public string? Anomaly { get; set; }
        public string? HOEStandardTimes { get; set; }
        public string? ModelsSpecification { get; set; }
        public string? Cycles { get; set; }

        public string? SsvCommentary { get; set; }
        public string? OperatorCommentary { get; set; }
        public string? SsvSignature { get; set; }
        public string? OperatorSignature { get; set; }
        public string? ReleasedFeedback { get; set; }
        public int? KpiId { get; set; }
        public string? TaktTime { get; set; }
        public string? Questions { get; set; }
        public int? ProductId { get; set; }

        public string? OperationTimesJson { get; set; }
        public string? StepsNumber { get; set; }
        public string? DoubleManagment { get; set; }
        public string? Waiting { get; set; }
    }
}
