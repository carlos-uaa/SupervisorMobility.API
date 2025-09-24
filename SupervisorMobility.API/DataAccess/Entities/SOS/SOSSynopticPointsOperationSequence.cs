using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSSynopticPointsOperationSequence
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSSynopticPointsOperationSequenceId { get; set; }
        public int? Sequence { get; set; }

        public int? SectionId { get; set; }
        public Section? Section { get; set; }

        public string? Times { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}