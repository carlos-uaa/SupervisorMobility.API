using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisDtos;

namespace SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos
{
    public class SectionForUpdateDto
    {
        public int SectionId { get; set; }
        public ICollection<AnalysisForUpdateDto> Analyses { get; set; } = new List<AnalysisForUpdateDto>();
        public string Step { get; set; } = "";
        public int SecuenceDist { get; set; }
        public bool? IsActive { get; set; }
    }
}
