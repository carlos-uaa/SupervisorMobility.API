using SupervisorMobility.API.DataAccess.Entities.LUP;
using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.ILU
{
    public class ILURegister
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ILURegisterid { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? AcquisitionDate { get; set; } = DateTime.Now;

        public int? OperationId { get; set; }
        [ForeignKey("OperationId")]
        public Operation? Operation { get; set; }


        public int? OperatorId { get; set; }
        [ForeignKey("OperatorId")]
        public User? Operator { get; set; }


        public int? ILULevelId { get; set; }
        [ForeignKey("OperatorId")]
        public ILULevel? ILULevel { get; set; }

        public bool isActive { get; set; }

    }
}
