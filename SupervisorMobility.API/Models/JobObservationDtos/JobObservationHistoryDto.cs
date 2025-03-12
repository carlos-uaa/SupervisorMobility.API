using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.ChecklistAnswerDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.LupDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.JobObservationDtos
{
    public class JobObservationHistoryDto
    {
        public int JobObservationVersionId { get; set; }
        public DateTime? DateModification { get; set; }
        public string? resumeVersion { get; set; }
        public string? MadeBy { get; set; }

        public PlantDto Plant { get; set; } = new PlantDto();
        public AreaWithoutNavigationPropertiesDto? Area { get; set; } = new AreaWithoutNavigationPropertiesDto();
        public DistributionWithoutNavigationPropertiesDto Distribution { get; set; } = new DistributionWithoutNavigationPropertiesDto();
        //public OperationWithoutNavigationPropertiesDto Operation { get; set; } = new OperationWithoutNavigationPropertiesDto();
        public UsersWithoutNavigationWithoutPeopleDetails Supervisor { get; set; } = new UsersWithoutNavigationWithoutPeopleDetails();
        public UsersWithoutPeopleWithNavigation? Operator { get; set; } = new UsersWithoutPeopleWithNavigation();
        //whitLup
        public ICollection<LupDto> Lup { get; set; } = new List<LupDto>();
        //Whit History
        public ICollection<OperationWithoutNavigationPropertiesDto>? Operations { get; set; } = new List<OperationWithoutNavigationPropertiesDto>();
        //Whit  answers to questions
        public ICollection<ChecklistAnswerDto>? checklistAnswers { get; set; } = new List<ChecklistAnswerDto>();


        public int JobObservationId { get; set; }
        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? DistributionId { get; set; }
        //public int? OperationId { get; set; }
        public int? SupervisorId { get; set; }
        public int? OperatorId { get; set; }
        public int? Type { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? FinishedDate { get; set; }

        public string? Justification { get; set; }
        public string? SectionIds { get; set; }
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

        public string? ProductIds { get; set; }
        public string? ProductSpecifications { get; set; }

        public string? OperationTimesJson { get; set; }
        public string? StepsNumber { get; set; }
        public string? DoubleManagment { get; set; }
        public string? Waiting { get; set; }

    }
}
