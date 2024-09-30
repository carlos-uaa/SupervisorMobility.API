using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSCombinationOperationSequence
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSCombinationOperationSequenceId { get; set; }
        public int? SequenceId { get; set; }
        public string? ProcessName { get; set; }
        public string? PartsPerCycle { get; set; }
        public string? ManualOperationTime { get; set; }
        public string? ManualOperationTimeWithMachineInAutomatic { get; set; }
        public string? AutomaticMachineOperationTime { get; set; }
        public bool? IsActive { get; set; }

    }
}
