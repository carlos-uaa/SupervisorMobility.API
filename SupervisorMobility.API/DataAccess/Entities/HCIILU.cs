using SupervisorMobility.API.DataAccess.Entities.ILU;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class HCIILU
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? Start {  get; set; }
        [Column(TypeName = "Date")]
        public DateTime? End { get; set; }
        public string Description { get; set; }
        public string level { get; set; }

        //public HCI? _HCI { get; set; }
        public int? RegisterILURegisterid { get; set; }
        public ILURegister? Register { get; set; }
    }
}
