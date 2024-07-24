using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos
{
    public class SOSAnalysisLogbookForCreateDto
    {

        public int SOSAnalysisLogbookId { get; set; }
        public int? Status { get; set; }
        public int? NoRevision { get; set; }
        public bool? IsActive { get; set; }

        public int SOSAnalysisId { get; set; }

        public string? RevisedItem { get; set; }

        public int? SeniorSupervisorId { get; set; }

        public int? SupervisorId { get; set; }

        public DateTime? Date { get; set; }
    }
}
