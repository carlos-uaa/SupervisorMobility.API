using SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;

namespace SupervisorMobility.API.Models.SOS.SOSSequenceDtos
{
    public class SOSSequenceForCreateDto
    {
        public int SOSSequenceId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }
        public ICollection<SOSSequenceLogbookForCreateDto>? SequenceLogbooks { get; set; } = new List<SOSSequenceLogbookForCreateDto>();
        public ICollection<CreateCommentaryDto>? Notes { get; set; } = new List<CreateCommentaryDto>();

        public DateTime? CreatedDate { get; set; }

        public bool? IsActive { get; set; }

        public int? SOSHubId { get; set; }
    }
}
