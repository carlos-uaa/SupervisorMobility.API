namespace SupervisorMobility.API.Models.SOS.SOSDistributionOperationSequenceDtos
{
    public class SOSDistributionOperationSequenceForCreateDto
    {
        public int? SequenceId { get; set; }

        public int? SectionId { get; set; }
      
        public string? Times { get; set; } = "";
        public bool? IsActive { get; set; }

    }
}
