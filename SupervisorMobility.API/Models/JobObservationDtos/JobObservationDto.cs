using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.JobObservationTypeDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.JobObservationDtos
{
    public class JobObservationDto
    {

        public PlantDto Plant { get; set; } = new PlantDto();
        public AreaWithoutNavigationPropertiesDto? Area { get; set; } = new AreaWithoutNavigationPropertiesDto();
        public DistributionWithoutNavigationPropertiesDto Distribution { get; set; } = new DistributionWithoutNavigationPropertiesDto();
        public OperationWithoutNavigationPropertiesDto Operation { get; set; } = new OperationWithoutNavigationPropertiesDto();

        public UsersWhitoutNavigationDetails Supervisor { get; set; } = new UsersWhitoutNavigationDetails();
        public UsersWhitoutNavigationDetails Operator { get; set; } = new UsersWhitoutNavigationDetails();


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
        public DateTime? EditStartDate { get; set; }
        public DateTime? EditEndDate { get; set; }
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
