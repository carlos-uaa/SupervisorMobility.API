
using SupervisorMobility.API.Models.SOS.TurnDtos;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSCombinationLogbookDtos;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Models.SOS.SOSCombinationDtos
{
    public class SOSCombinationForCreateDto
    {
        public int SOSCombinationId { get; set; }
        public bool? IsActive { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }


        public int? ReviewerHSId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails? ReviewerHS { get; set; }
        public FileUploadGeneralDto? ReviewerHSSignatureImage { get; set; } = new();

      

        public ICollection<TurnForCreateDto>? Turns { get; set; }

        public DateTime? ApplicationMonth { get; set; }

        public string? ProductionVolumePerShift { get; set; }
        public string? TackTime { get; set; }
        public string? ControlNumber { get; set; }
        public ICollection<SOSCombinationLogbookForCreateDto>? CombinationLogbooks { get; set; } = new List<SOSCombinationLogbookForCreateDto>();
        public ICollection<FileUploadForCreationDto>? Illustrations { get; set; } = new List<FileUploadForCreationDto>();

        public int SOSHubId { get; set; }
    }
}
