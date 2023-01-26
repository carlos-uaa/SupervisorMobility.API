using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class AssyChart
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssyChardId { get; set; }
       
        //Navigation properties
        public int PlantId { get; set; }
        public Plant? Plant { get; set; }
        
        public int AreaId { get; set; }
        public Area? Area { get; set; }

        public int DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public int OperacionId { get; set; }
        public Operation? Operation { get; set; }

        public bool? IsActive { get; set; }

        public string GOS { get; set; }
        public string CCP { get; set; }
        public string HCE { get; set; }
        public string Modelo { get; set; }


    }

}
