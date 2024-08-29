using SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;

namespace SupervisorMobility.API.Models.SOS.SOSSequenceDtos
{
    public class SOSSequenceDto
    {
        public int SOSSequenceId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }

        public ICollection<SOSSequenceLogbookDto>? SequenceLogbooks { get; set; } = new List<SOSSequenceLogbookDto>();
        public ICollection<FileUploadGeneralDto>? Illustrations { get; set; } = new List<FileUploadGeneralDto>();
        public ICollection<CommentaryDto>? Notes { get; set; } = new List<CommentaryDto>();
        public ICollection<SOSTimeDto>? Times { get; set; } = new List<SOSTimeDto>();

        public DateTime? CreatedDate { get; set; }

        public bool? IsActive { get; set; }

        public int? SOSHubId { get; set; }
        public SOSHubDto? SOSHub { get; set; }
    }
}
