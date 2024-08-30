using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSFlowDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.SOSFlowLogbookDtos
{
    public class SOSFlowLogbookDto
    {
        public int SOSFlowLogbookId { get; set; }
        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int SOSFlowId { get; set; }
        public SOSFlowDto? SOSFlow { get; set; }

        public int? ApproverId { get; set; }
        public UsersWithNavigationAndPeopleDetails? Approver { get; set; }
        public FileUploadGeneralDto? ApproverSignatureImage { get; set; } = new();

        public int? ReviewerId { get; set; }
        public UsersWithNavigationAndPeopleDetails? Reviewer { get; set; }
        public FileUploadGeneralDto? ReviewerSignatureImage { get; set; } = new();
    }
}
