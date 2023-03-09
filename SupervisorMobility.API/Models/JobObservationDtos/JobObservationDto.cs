using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.JobObservationTypeDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.PlantDtos;

namespace SupervisorMobility.API.Models.JobObservationDtos
{
    public class JobObservationDto
    {

        public PlantDto Plant { get; set; } = new PlantDto();
        public AreaWithoutNavigationPropertiesDto? Area { get; set; } = new AreaWithoutNavigationPropertiesDto();
        public DistributionWithoutNavigationPropertiesDto Distribution { get; set; } = new DistributionWithoutNavigationPropertiesDto();
        public OperationWithoutNavigationPropertiesDto Operation { get; set; } = new OperationWithoutNavigationPropertiesDto();

        public int JobObservationId { get; set; }
        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? DistributionId { get; set; }
        public int? OperationId { get; set; }

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

        public string? SArea { get; set; }
        public string? QArea { get; set; }
        public string? DArea { get; set; }
        public string? CArea { get; set; }
        public string? OthersArea { get; set; }

        public string? IdentifiedActivity { get; set; }
        public string? SsvCommentary { get; set; }
        public string? OperatorCommentary { get; set; }
        public string? SsvSignature { get; set; }
        public string? OperatorSignature { get; set; }

    }
}
