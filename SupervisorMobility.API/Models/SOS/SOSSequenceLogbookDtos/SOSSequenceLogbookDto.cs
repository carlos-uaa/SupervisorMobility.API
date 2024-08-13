using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos
{
    public class SOSSequenceLogbookDto
    {

        public int SOSSequenceLogbookId { get; set; }
        public int? Status { get; set; }
        public int? NoRevision { get; set; }
        public bool? IsActive { get; set; }

        public int SOSSequenceId { get; set; }
        public SOSSequenceDto? SOSSequence { get; set; }

        public string? RevisedItem { get; set; }

        public int? SeniorSupervisorId { get; set; }
        public UsersWithNavigationAndPeopleDetails? SeniorSupervisor { get; set; }
        public FileUploadGeneralDto? SeniorSupervisorSignatureImage { get; set; } = new();

        public int? SupervisorId { get; set; }
        public UsersWithNavigationAndPeopleDetails? Supervisor { get; set; }
        public FileUploadGeneralDto? SupervisorSignatureImage { get; set; } = new();

        public DateTime? Date { get; set; }
    }
}
