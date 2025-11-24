namespace SupervisorMobility.API.Models.SOS.SOSCombinationOperationSequenceDtos
{
    public class SOSCombinationOperationSequenceDto
    {
        public int SOSCombinationOperationSequenceId { get; set; }
        public int? SequenceId { get; set; }
        public int? SectionId { get; set; }
        public string? ProcessName { get; set; }
        public string? PartsPerCycle { get; set; }
        public double? ManualOperationTime { get; set; }
        public double? ManualOperationTimeWithMachineInAutomatic { get; set; }
        public double? AutomaticMachineOperationTime { get; set; }
        public double? StepsToNextProcess { get; set; }
        public bool? IsActive { get; set; }
    }
}
