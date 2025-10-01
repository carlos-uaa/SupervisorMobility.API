using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSSynopticRequirementsOperationSequence
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSSynopticRequirementsOperationSequenceId { get; set; }
        public int? Sequence { get; set; }
        public int? SectionId { get; set; }
        public int? SosHubId { get; set; }
        public Section? Section { get; set; }
        public string? OperationPersonText { get; set; } = "";
        public string? OperationMachineText { get; set; } = "";
        public bool? IsOperationPersonRequired { get; set; } = true;
        public bool? IsOperationMachineRequired { get; set; } = false;
        public string? Times { get; set; } = "";
        public bool? IsActive { get; set; }
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public SOSSynopticTableofOperatingRequirements? SOSSynopticTableofOperatingRequirements { get; set; }
    }
}