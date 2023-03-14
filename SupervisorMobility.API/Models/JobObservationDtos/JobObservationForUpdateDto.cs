using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.JobObservationDtos
{
    public class JobObservationForUpdateDto
    {
        public bool? IsActive { get; set; }

        public int PlantId { get; set; }
        public int AreaId { get; set; }
        public int DistributionId { get; set; }
        public int OperationId { get; set; }

        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        public DateTime? DateFinalized { get; set; }

        public string? Justification { get; set; }

        public int? Status { get; set; }

        public string? Observer { get; set; }
        public string? Operator { get; set; }

        public int? Option { get; set; }
        public string? Anomaly { get; set; }


        public string? Time1HOE { get; set; }
        public string? Time2HOE { get; set; }
        public string? Models { get; set; }
        public string? Cicles { get; set; }

        public string? IdentifiedActivity { get; set; }
        public string? SsvCommentary { get; set; }
        public string? OperatorCommentary { get; set; }
        public string? SsvSignature { get; set; }
        public string? OperatorSignature { get; set; }
    }
}
