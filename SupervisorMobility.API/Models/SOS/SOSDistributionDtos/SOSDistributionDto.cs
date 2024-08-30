using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos;

namespace SupervisorMobility.API.Models.SOS.SOSDistributionDtos
{
    public class SOSDistributionDto
    {
        public int SOSDistributionId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }
        public int? ReviewerId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails? Reviewer { get; set; }
        public FileUploadGeneralDto? ReviewerSignatureImage { get; set; } = new();

        public int? ApproverId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails? Approver { get; set; }
        public FileUploadGeneralDto? ApproverSignatureImage { get; set; } = new();

        public string? TackTime { get; set; }

        public ICollection<TurnDto>? Turns { get; set; }
        public string? AplicationModels { get; set; } = "§§§§";
        public ICollection<SOSTimeDto>? AplicationModelsTimes { get; set; }

        public ICollection<SOSDistributionLogbookDto>? DistributionLogbooks { get; set; } = new List<SOSDistributionLogbookDto>();
        public ICollection<FileUploadGeneralDto>? Illustrations { get; set; } = new List<FileUploadGeneralDto>();
        public ICollection<CommentaryDto>? Notes { get; set; } = new List<CommentaryDto>();

        public string? AdditionalTime { get; set; } = "§§§§";
        public string? CycleTime { get; set; } = "§§§§";
        public string? ControlNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ApplicationMonth { get; set; }

        public bool? IsActive { get; set; }

        public int? SOSHubId { get; set; }
        public SOSHubDto? SOSHub { get; set; }
    }
}
