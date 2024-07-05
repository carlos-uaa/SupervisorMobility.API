using SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SpecialCaseAbnormalSituationDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;

namespace SupervisorMobility.API.Models.SOS.SOSAnalysisDtos
{
    public class SOSAnalysisDto
    {
        public int SOSAnalysisId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? ProcessNumber { get; set; }

        ICollection<SOSAnalysisLogbookDto>? AnalysisLogbooks { get; set; } = new List<SOSAnalysisLogbookDto>();
        ICollection<FileUploadGeneralDto>? Illustrations { get; set; } = new List<FileUploadGeneralDto>();
        ICollection<CommentaryDto>? Notes { get; set; } = new List<CommentaryDto>();
        ICollection<SpecialCaseAbnormalSituationDto>? SpecialCasesAbnormalSituations { get; set; } = new List<SpecialCaseAbnormalSituationDto>();

        public DateTime? CreatedDate { get; set; }

        public bool? IsActive { get; set; }

        public int SOSHubId { get; set; }
        public SOSHubDto? SOSHub { get; set; }
    }
}
