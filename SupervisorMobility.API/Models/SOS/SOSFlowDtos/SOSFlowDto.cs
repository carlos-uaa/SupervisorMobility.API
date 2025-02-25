using SupervisorMobility.API.Models.SOS.SOSFlowLogbookDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.SOSFlowDtos
{
    public class SOSFlowDto
    {
        public int SOSFlowId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }

        public string Flow { get; set; }

        public int? ReviewerHSId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails? ReviewerHS { get; set; }
        public FileUploadGeneralDto? ReviewerHSSignatureImage { get; set; } = new();

     
        public DateTime? CreatedAt { get; set; }
        public string? TargetTime { get; set; }

        public ICollection<SOSFlowLogbookDto>? FlowLogbooks { get; set; } = new List<SOSFlowLogbookDto>();


        public bool? IsActive { get; set; }
        public int SOSHubId { get; set; }
        public SOSHubDto? SOSHub { get; set; }

    }
}
