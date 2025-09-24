using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos;
using System.Text.Json.Serialization;

namespace SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsDtos
{
    public class SOSSynopticTableofOperatingRequirementsForCreateDto
    {
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? ProcessName { get; set; }
        public int? CreatorId { get; set; }
        public int? ReviewerId { get; set; }
        public int? ApproverId { get; set; }
        public DateTime? CreatedAt { get; set; }
        [JsonIgnore]
        public List<SOSSynopticRequirementsOperationSequenceForCreateDto>? SOSSynopticRequirementsOperationSequence { get; set; }
        [JsonIgnore]
        public List<SOSSynopticRequirementsLogbookForCreateDto>? SynopticRequirementsLogbooks { get; set; }
        [JsonIgnore]
        public List<SOSSynopticTableRequirementOperationDifficulty>? RequirementDifficulties { get; set; } = new List<SOSSynopticTableRequirementOperationDifficulty>();
        [JsonIgnore]
        public List<SOSAnalysisDto>? Analyses { get; set; } 
        [JsonIgnore]
        public List<SOSSequenceDto>? Sequences { get; set; } 

        public bool? IsActive { get; set; }
        public int SOSHubId { get; set; }
    }
}
