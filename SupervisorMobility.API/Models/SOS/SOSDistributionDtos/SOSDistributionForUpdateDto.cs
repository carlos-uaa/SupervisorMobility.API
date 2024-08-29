
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;

namespace SupervisorMobility.API.Models.SOS.SOSDistributionDtos
{
    public class SOSDistributionForUpdateDto
    {
        public int SOSDistributionId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }
        public int? ReviewerId { get; set; }

        public int? ApproverId { get; set; }

        public string? TackTime { get; set; }

        public List<TurnForUpdateDto>? Turns { get; set; }
        public string? AplicationModels { get; set; } = "§§§§";
        public List<SOSTimeForUpdateDto>? AplicationModelsTimes { get; set; }

        public List<SOSDistributionLogbookForUpdateDto>? DistributionLogbooks { get; set; } = new List<SOSDistributionLogbookForUpdateDto>();
        public ICollection<FileUploadGeneralDto>? Illustrations { get; set; } = new List<FileUploadGeneralDto>();
        public List<UpdateCommentaryDto>? Notes { get; set; } = new List<UpdateCommentaryDto>();

        public string? AdditionalTime { get; set; } = "§§§§";
        public string? CycleTime { get; set; } = "§§§§";
        public string? ControlNumber { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ApplicationMonth { get; set; }

        public bool? IsActive { get; set; }

        public int? SOSHubId { get; set; }
    }
}
