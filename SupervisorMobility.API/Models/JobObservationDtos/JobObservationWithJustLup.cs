using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.JobObservationTypeDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.LupDtos;

namespace SupervisorMobility.API.Models.JobObservationDtos
{
    public class JobObservationWithJustLupDto
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
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        public DateTime? DateFinalized { get; set; }

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
