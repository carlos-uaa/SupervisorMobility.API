using SupervisorMobility.API.DataAccess.Entities.LUP;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class LeadershipRecord
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeadershipRecordsid { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? AcquisitionDate { get; set; } = DateTime.Now;

        public int? DistributionId { get; set; }
        [ForeignKey("DistributionId")]
        public Distribution? Distribution { get; set; }

        public int? OperatorId { get; set; }
        [ForeignKey("OperatorId")]
        public User? Operator { get; set; }

        public int? ILULevelId { get; set; }
        [ForeignKey("ILULevelId")]
        public ILULevel? ILULevel { get; set; }

        public bool isActive { get; set; }
    }
}
