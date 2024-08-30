using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class Equipment
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentId { get; set; }
        public string EquipmentCode { get; set; }
        public string EquipmentName { get; set; }
        public bool? IsActive { get; set; }
        public ICollection<SOSHub>? SafetyEquipment { get; set; } = new List<SOSHub>();
        public ICollection<SOSHubHistory>? SafetyEquipmentHistory { get; set; } = new List<SOSHubHistory>();
    }
}
