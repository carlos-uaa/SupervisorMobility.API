using SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;

namespace SupervisorMobility.API.Models.SOS.SOSAnalysisDtos
{
    public class SOSAnalysisForUpdateDto
    {
        public int SOSAnalysisId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }

        public List<SOSAnalysisLogbookForUpdateDto>? AnalysisLogbooks { get; set; } = new List<SOSAnalysisLogbookForUpdateDto>();
        public List<UpdateCommentaryDto>? Notes { get; set; } = new List<UpdateCommentaryDto>();
        public List<SOSTimeForUpdateDto>? Times { get; set; } = new List<SOSTimeForUpdateDto>();

        public DateTime? CreatedDate { get; set; }

        public bool? IsActive { get; set; }

        public int? SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
    }
}
