using SupervisorMobility.API.Models.SOS.SOSFlowLogbookDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.SOSFlowDtos
{
    public class SOSFlowForCreateDto
    {
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }
        public int? ReviewerId { get; set; }
        public int? ReviewerHSId { get; set; }

        public int? ApproverId { get; set; }
        public string? TargetTime { get; set; }
        public ICollection<SOSFlowLogbookForCreateDto>? FlowLogbooks { get; set; } = new List<SOSFlowLogbookForCreateDto>();


        public bool? IsActive { get; set; }
        public int SOSHubId { get; set; }
        public SOSHubDto? SOSHub { get; set; }
    }
}
