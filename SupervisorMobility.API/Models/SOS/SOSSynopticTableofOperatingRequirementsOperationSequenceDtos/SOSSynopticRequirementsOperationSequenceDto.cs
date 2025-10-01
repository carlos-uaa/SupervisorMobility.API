using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;

namespace SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos
{
    public class SOSSynopticRequirementsOperationSequenceDto
    {
        public int SOSSynopticRequirementsOperationSequenceId { get; set; }
        public int? Sequence { get; set; }
        public int? SectionId { get; set; }
        public int? SosHubId { get; set; }
        public SectionDto? Section { get; set; }
        public string? OperationPersonText { get; set; } = "";
        public string? OperationMachineText { get; set; } = "";
        public bool? IsOperationPersonRequired { get; set; } = true;
        public bool? IsOperationMachineRequired { get; set; } = false;
        public string? Times { get; set; } = "";
        public bool? IsActive { get; set; }
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
    }
}
