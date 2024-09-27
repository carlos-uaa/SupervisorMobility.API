using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationOperationSequenceDtos;

namespace SupervisorMobility.API.Models.SOS.SOSCombinationDtos
{
    public class SOSCombinationDto
    {
        public int SOSCombinationId { get; set; }
        public bool? IsActive { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }
 

        public int? ReviewerHSId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails? ReviewerHS { get; set; }
        public FileUploadGeneralDto? ReviewerHSSignatureImage { get; set; } = new();


        public ICollection<TurnDto>? Turns { get; set; }
        public DateTime? ApplicationMonth { get; set; }

        public string? ProductionVolumePerShift { get; set; }
        public string? TackTime { get; set; }
        public string? ControlNumber { get; set; }

        public ICollection<SOSCombinationLogbookDto>? CombinationLogbooks { get; set; } = new List<SOSCombinationLogbookDto>();
        public ICollection<FileUploadGeneralDto>? Illustrations { get; set; } = new List<FileUploadGeneralDto>();

        public DateTime? CreatedAt { get; set; }
        public int? SOSCombinationOperationSequenceId { get; set; }
        public ICollection<SOSCombinationOperationSequenceDto>? SOSCombinationOperationSequence { get; set; }

        public int SOSHubId { get; set; }
        public SOSHubDto? SOSHub { get; set; }
    }
}
