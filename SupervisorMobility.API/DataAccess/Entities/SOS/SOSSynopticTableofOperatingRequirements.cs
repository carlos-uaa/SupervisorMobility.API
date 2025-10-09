// - Core .NET imports
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// - Custom project imports
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;


namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSSynopticTableofOperatingRequirements
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }

        public string? InternalControlNumber { get; set; }
        public string? ProcessName { get; set; }


        public int? CreatorId { get; set; }
        public User? Creator { get; set; }
        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public int? ApproverId { get; set; }
        public User? Approver { get; set; }


        public DateTime? CreatedAt { get; set; }


        public ICollection<SOSSynopticRequirementsOperationSequence>? SOSSynopticRequirementsOperationSequence { get; set; }

        public ICollection<SOSSynopticRequirementsLogbook>? SynopticRequirementsLogbooks { get; set; } = new List<SOSSynopticRequirementsLogbook>();

        public ICollection<SOSSynopticTableRequirementOperationDifficulty>? RequirementDifficulties { get; set; } = new List<SOSSynopticTableRequirementOperationDifficulty>();

        public bool? IsActive { get; set; }
        public int? SOSHubId { get; set; }

        public ICollection<SOSHub>? SOSHubs { get; set; } = new List<SOSHub>();
        //las analisis y las secuencias de las que se sacaran los puntos principal
        public ICollection<SOSAnalysis>? Analyses { get; set; } = new List<SOSAnalysis>();
        public ICollection<SOSSequence>? Sequences { get; set; } = new List<SOSSequence>();

        public ICollection<SOSSTROKnowledgeHub>? SOSSTROKnowledge { get; set; } = new List<SOSSTROKnowledgeHub>();
        public ICollection<SOSSTROSkillHub>? SOSSTROSkill { get; set; } = new List<SOSSTROSkillHub>();
        public ICollection<EstablishedConditions>? EstablishedConditions { get; set; } = new List<EstablishedConditions>();
        public ICollection<InsuranceFeatures>? InsuranceFeatures { get; set; } = new List<InsuranceFeatures>();
    }
}