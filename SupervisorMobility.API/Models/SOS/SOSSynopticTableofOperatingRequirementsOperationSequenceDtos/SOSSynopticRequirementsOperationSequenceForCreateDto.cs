namespace SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos
{
    public class SOSSynopticRequirementsOperationSequenceForCreateDto
    {
        public int? Sequence { get; set; }

        public int? SectionId { get; set; }

        public string? Times { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}
