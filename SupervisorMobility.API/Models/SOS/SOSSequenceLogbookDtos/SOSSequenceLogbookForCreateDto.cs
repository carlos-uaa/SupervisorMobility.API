using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos
{
    public class SOSSequenceLogbookForCreateDto
    {

        public int SOSSequenceLogbookId { get; set; }
        public int? Status { get; set; }
        public int? NoRevision { get; set; }
        public bool? IsActive { get; set; }

        public int SOSSequenceId { get; set; }

        public string? RevisedItem { get; set; }

        public int? SeniorSupervisorId { get; set; }

        public int? SupervisorId { get; set; }

        public DateTime? Date { get; set; }
    }
}
