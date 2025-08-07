using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;

namespace SupervisorMobility.API.Models.SOS.SOSDistributionOperationSequenceDtos
{
    public class SOSDistributionOperationSequenceDto
    {
        public int SOSDistributionOperationSequenceId { get; set; }
        public int? SequenceId { get; set; }

        public int? SectionId { get; set; }
        public SectionDto? Section { get; set; }

        public string? Times { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}
