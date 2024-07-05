using SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SpecialCaseAbnormalSituationDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;

namespace SupervisorMobility.API.Models.SOS.SOSAnalysisDtos
{
    public class SOSAnalysisForUpdateDto
    {
        public int SOSAnalysisId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }

        ICollection<SOSAnalysisLogbookDto>? AnalysisLogbooks { get; set; } = new List<SOSAnalysisLogbookDto>();
        ICollection<CommentaryDto>? Notes { get; set; } = new List<CommentaryDto>();
        ICollection<SpecialCaseAbnormalSituationDto>? SpecialCasesAbnormalSituations { get; set; } = new List<SpecialCaseAbnormalSituationDto>();

        public DateTime? CreatedDate { get; set; }

        public bool? IsActive { get; set; }

        public int SOSHubId { get; set; }
    }
}
