using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.FileUploadDto;

namespace SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos
{
    public class SOSAnalysisLogbookForUpdateDto
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
