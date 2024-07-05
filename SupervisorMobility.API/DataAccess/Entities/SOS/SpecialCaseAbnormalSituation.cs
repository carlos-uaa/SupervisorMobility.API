using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.DataAccess.Entities.IS;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SpecialCaseAbnormalSituation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SpecialCaseAbnormalSituationId { get; set; }
        public string key { get; set; }

        public string PartName { get; set; }
        public string PartNumber { get; set; }

        public int? PartId { get; set; }
        public Part? Part { get; set; }

        public double Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}
