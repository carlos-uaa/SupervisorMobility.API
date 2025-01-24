namespace SupervisorMobility.API.Models.SOS.SOSCombinationOperationSequenceDtos
{
    public class SOSCombinationOperationSequenceDto
    {
        public int SOSCombinationOperationSequenceId { get; set; }
        public int? SequenceId { get; set; }
        public int? SectionId { get; set; }
        public string? ProcessName { get; set; }
        public string? PartsPerCycle { get; set; }
        public string? ManualOperationTime { get; set; }
        public string? ManualOperationTimeWithMachineInAutomatic { get; set; }
        public string? AutomaticMachineOperationTime { get; set; }
        public string? StepsToNextProcess { get; set; }
        public bool? IsActive { get; set; }
    }
}
