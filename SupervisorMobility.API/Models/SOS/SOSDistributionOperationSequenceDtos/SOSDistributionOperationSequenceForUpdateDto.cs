namespace SupervisorMobility.API.Models.SOS.SOSDistributionOperationSequenceDtos
{
    public class SOSDistributionOperationSequenceForUpdateDto
    {
        public int SOSDistributionOperationSequenceId { get; set; }
        public int? SequenceId { get; set; }

        public int? SectionId { get; set; }

        public string? Times { get; set; } = "";
        public bool? IsActive { get; set; }

    }
}
