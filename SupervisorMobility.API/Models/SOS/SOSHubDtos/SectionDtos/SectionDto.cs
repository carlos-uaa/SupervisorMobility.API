using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisDtos;

namespace SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos
{
    public class SectionDto
    {
        public int SectionId { get; set; }
        public ICollection<AnalysisDto> Analyses { get; set; } = new List<AnalysisDto>();
        public string Step { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}
