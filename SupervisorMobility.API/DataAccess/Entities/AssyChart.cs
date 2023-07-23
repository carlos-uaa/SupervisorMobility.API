using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class AssyChart
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssyChardId { get; set; }
        public bool? IsActive { get; set; }
        public string? GOS { get; set; } = string.Empty;
        public string? CCP { get; set; } = string.Empty;
        public string? HOE { get; set; } = string.Empty;

        [Column(TypeName = "Date")]
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime ModificationDate { get; set; }

        //Navigation properties
        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        //arbol
        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }

        public int? AreaId { get; set; }
        public Area? Area { get; set; }

        public int? DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public int? OperationId { get; set; }
        public Operation? Operation { get; set; }


    }

}

