namespace SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos
{
    public class SOSSynopticRequirementsOperationSequenceForUpdateDto
    {   
        public int SOSSynopticRequirementsOperationSequenceId { get; set; }
        public int? Sequence { get; set; }
        
        public int? SectionId { get; set; }

        public string? Times { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}
