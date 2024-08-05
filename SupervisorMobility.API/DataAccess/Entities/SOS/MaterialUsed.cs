using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.DataAccess.Entities.IS;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class MaterialUsed
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaterialUsedId { get; set; }
        
        public int MaterialId { get; set; }
        public Material Material { get; set; }

        public double Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}
