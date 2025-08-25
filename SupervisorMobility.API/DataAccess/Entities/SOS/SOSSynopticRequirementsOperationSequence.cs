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
        public Section? Section { get; set; }

        public string? Times { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}