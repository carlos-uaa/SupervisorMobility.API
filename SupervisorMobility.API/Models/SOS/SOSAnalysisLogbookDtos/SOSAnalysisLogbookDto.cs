using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos
{
    public class SOSAnalysisLogbookDto
    {

        public int SOSAnalysisLogbookId { get; set; }
        public int? Status { get; set; }
        public int? NoRevision { get; set; }
        public bool? IsActive { get; set; }

        public int SOSAnalysisId { get; set; }
        public SOSAnalysisDto? SOSAnalysis { get; set; }

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
