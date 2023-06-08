using SupervisorMobility.API.Models.LupDtos;

namespace SupervisorMobility.API.Models.JobObservationDtos
{
    public class JobObservationWithJustLupDto
    {

        public ICollection<LupDto> Lup { get; set; } = new List<LupDto>();
        public ICollection<JobObservationHistoryDto> History { get; set; } = new List<JobObservationHistoryDto>();

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


        public string? Time1HOE { get; set; }
        public string? Time2HOE { get; set; }
        public string? Models { get; set; }
        public string? Cicles { get; set; }

        public string? SsvCommentary { get; set; }
        public string? OperatorCommentary { get; set; }
        public string? SsvSignature { get; set; }
        public string? OperatorSignature { get; set; }

    }
}
