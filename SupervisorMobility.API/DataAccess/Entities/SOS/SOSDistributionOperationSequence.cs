using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSDistributionOperationSequence
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSDistributionOperationSequenceId { get; set; }
        public int? SequenceId { get; set; }

        public int? SectionId { get; set; }
        public Section? Section { get; set; }

        public bool IsAnalysis { get; set; }
        public string? Times { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}