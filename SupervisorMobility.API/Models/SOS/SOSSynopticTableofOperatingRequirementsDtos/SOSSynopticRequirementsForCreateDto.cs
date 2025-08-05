using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos;

namespace SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsDtos
{
    public class SOSSynopticRequirementsForCreateDto
    {
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? ProcessName { get; set; }
        public int? CreatorId { get; set; }
        public int? ReviewerId { get; set; }
        public int? ApproverId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public ICollection<SOSSynopticRequirementsOperationSequenceForCreateDto>? SOSSynopticRequirementsOperationSequence { get; set; }
        public ICollection<SOSSynopticRequirementsLogbookForCreateDto>? SynopticRequirementsLogbooks { get; set; } = new List<SOSSynopticRequirementsLogbookForCreateDto>();
        public bool? IsActive { get; set; }
        public int SOSHubId { get; set; }
    }
}
