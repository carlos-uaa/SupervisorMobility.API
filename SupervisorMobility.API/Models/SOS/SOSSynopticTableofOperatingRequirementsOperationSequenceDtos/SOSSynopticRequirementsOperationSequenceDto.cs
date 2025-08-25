using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;

namespace SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos
{
    public class SOSSynopticRequirementsOperationSequenceDto
    {
        public int SOSSynopticRequirementsOperationSequenceId { get; set; }
        public int? Sequence { get; set; }

        public int? SectionId { get; set; }
        public SectionDto? Section { get; set; }

        public string? Times { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}
