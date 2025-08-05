using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsLogbookDtos
{
    public class SOSSynopticRequirementsLogbookDto
    {
        public int SOSSynopticRequirementsLogbookId { get; set; }
        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int SOSSynopticRequirementsId { get; set; }
        public SOSSynopticRequirementsDto? SOSSynopticRequirements { get; set; }


        public int? ApproverId { get; set; }
        public UsersWithNavigationAndPeopleDetails? Approver { get; set; }
        public FileUploadGeneralDto? ApproverSignatureImage { get; set; } = new();

        public int? ReviewerId { get; set; }
        public UsersWithNavigationAndPeopleDetails? Reviewer { get; set; }
        public FileUploadGeneralDto? ReviewerSignatureImage { get; set; } = new();
    }
}
