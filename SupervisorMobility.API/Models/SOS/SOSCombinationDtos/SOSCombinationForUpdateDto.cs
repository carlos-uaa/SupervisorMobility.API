
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSCombinationLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.SOSCombinationDtos
{
    public class SOSCombinationForUpdateDto
    {
        public int SOSCombinationId { get; set; }
        public bool? IsActive { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }
   

        public int? ReviewerHSId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails? ReviewerHS { get; set; }
        public FileUploadGeneralDto? ReviewerHSSignatureImage { get; set; } = new();

      

        public List<TurnForUpdateDto>? Turns { get; set; }

        public DateTime? ApplicationMonth { get; set; }

        public string? ProductionVolumePerShift { get; set; }
        public string? TackTime { get; set; }
        public string? ControlNumber { get; set; }
        public List<SOSCombinationLogbookForUpdateDto>? CombinationLogbooks { get; set; } = new List<SOSCombinationLogbookForUpdateDto>();
        public ICollection<FileUploadGeneralDto>? Illustrations { get; set; } = new List<FileUploadGeneralDto>();


        public int SOSHubId { get; set; }
        public SOSHubDto? SOSHub { get; set; }
    }
}
