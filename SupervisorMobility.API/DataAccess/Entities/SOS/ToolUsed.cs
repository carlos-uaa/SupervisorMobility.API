using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.DataAccess.Entities.IS;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class ToolUsed
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ToolUsedId { get; set; }
        
        public int ToolId { get; set; }
        public Tool Tool { get; set; }

        public double Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}
