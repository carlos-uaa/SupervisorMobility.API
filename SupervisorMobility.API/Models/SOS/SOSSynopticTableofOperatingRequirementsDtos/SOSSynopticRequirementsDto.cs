// - Core .NET imports
using System.Text.Json.Serialization;

// - Entity imports
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;

// - DTO imports
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos;


namespace SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsDtos
{
    public class SOSSynopticRequirementsDto
    {
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }

        public string? InternalControlNumber { get; set; }
        public string? ProcessName { get; set; }


        public int? CreatorId { get; set; }

        public UsersWithNavigationAndPeopleDetails? Creator { get; set; }
        public int? ReviewerId { get; set; }
        public UsersWithNavigationAndPeopleDetails? Reviewer { get; set; }
        public int? ApproverId { get; set; }
        public UsersWithNavigationAndPeopleDetails? Approver { get; set; }


        public DateTime? CreatedAt { get; set; }


     
        public ICollection<SOSSynopticRequirementsOperationSequenceDto>? SOSSynopticRequirementsOperationSequence { get; set; }

      
        public ICollection<SOSSynopticRequirementsLogbookDto>? SynopticRequirementsLogbooks { get; set; } = new List<SOSSynopticRequirementsLogbookDto>();

        public ICollection<SOSSynopticTableRequirementOperationDifficulty>? RequirementDifficulties { get; set; } = new List<SOSSynopticTableRequirementOperationDifficulty>();

        public bool? IsActive { get; set; }
        public int? SOSHubId { get; set; }

        
        public IEnumerable<SOSHubDto>? SOSHubs { get; set; } = new List<SOSHubDto>();
        //las analisis y las secuencias de las que se sacaran los puntos principal
     
        public IEnumerable<SOSAnalysisDto>? Analyses { get; set; } = new List<SOSAnalysisDto>();
     
        public IEnumerable<SOSSequenceDto>? Sequences { get; set; } = new List<SOSSequenceDto>();
        public IEnumerable<SOSSTROKnowledgeHub>? SOSSTROKnowledge { get; set; } = new List<SOSSTROKnowledgeHub>();
        public IEnumerable<SOSSTROSkillHub>? SOSSTROSkill { get; set; } = new List<SOSSTROSkillHub>();
    }
}
