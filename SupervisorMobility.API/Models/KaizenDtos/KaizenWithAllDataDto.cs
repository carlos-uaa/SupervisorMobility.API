using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.KaizenTransactionDtos;
using SupervisorMobility.API.Models.PillarDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.KaizenDtos
{
    public class KaizenWithAllDataDto
    {
        public int KaizenId { get; set; }

        public string KaizenName { get; set; }
        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        public PlantDto Plant { get; set; } = new PlantDto();
        public int? AreaId { get; set; }
        public AreaWithoutNavigationPropertiesDto? Area { get; set; } = new AreaWithoutNavigationPropertiesDto();

        public int PillarId { get; set; }
        public PillarDto? Pillar { get; set; }

        public int? SupervisorId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails Supervisor { get; set; } = new UsersWithoutNavigationWithoutPeopleDetails();

        public int? SeniorSupervisorId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails SeniorSupervisor { get; set; } = new UsersWithoutNavigationWithoutPeopleDetails();

        public int? ProposedId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails Proposed { get; set; } = new UsersWithoutNavigationWithoutPeopleDetails();

        public ICollection<FileUploadGeneralDto>? PreviousEvidences { get; set; }

        public ICollection<FileUploadGeneralDto>? ThenEvidences { get; set; }

        public ICollection<KaizenTransactionDto>? Transactions { get; set; }

        public string PreviousJustification { get; set; }

        public string ThenJustification { get; set; }

        public string StandardModification { get; set; }

        public string CalculationFormula { get; set; }

        public double? Total { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? FinishedDate { get; set; }
        public int Status { get; set; }
        public string kpiName { get; set; }
        public bool IsSignedSSV { get; set; }
        public bool IsSignedSupervisor { get; set; }
        public int OperatorPayroll { get; set; }

    }
}
