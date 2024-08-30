using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos
{
    public class SOSAnalysisLogbookForCreateDto
    {
        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int SOSAnalysisId { get; set; }

        public int? ApproverId { get; set; }
        public int? ReviewerId { get; set; }
    }
}
