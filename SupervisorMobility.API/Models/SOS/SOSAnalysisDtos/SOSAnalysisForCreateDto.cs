using SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;

namespace SupervisorMobility.API.Models.SOS.SOSAnalysisDtos
{
    public class SOSAnalysisForCreateDto
    {
        public int SOSAnalysisId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }
        public ICollection<SOSAnalysisLogbookDto>? AnalysisLogbooks { get; set; } = new List<SOSAnalysisLogbookDto>();
        public ICollection<CreateCommentaryDto>? Notes { get; set; } = new List<CreateCommentaryDto>();
        public List<SOSTimeForCreateDto>? Times { get; set; } = new List<SOSTimeForCreateDto>();

        public DateTime? CreatedDate { get; set; }

        public bool? IsActive { get; set; }

        public int? SOSHubId { get; set; }
    }
}
