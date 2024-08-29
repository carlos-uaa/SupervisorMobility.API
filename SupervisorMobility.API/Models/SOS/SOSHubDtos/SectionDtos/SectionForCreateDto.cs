using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisDtos;

namespace SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos
{
    public class SectionForCreateDto
    {
        public ICollection<AnalysisForCreateDto> Analyses { get; set; } = new List<AnalysisForCreateDto>();
        public string Step { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}
