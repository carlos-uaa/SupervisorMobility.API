using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Entities
{
    public class JobObservation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobObservationId { get; set; }
        public bool? IsActive { get; set; }

        public Plant? Plant { get; set; }
        public int? PlantId { get; set; }
        public Area? Area { get; set; }
        public int? AreaId { get; set; }
        public Distribution? Distribution { get; set; }
        public int? DistributionId { get; set; }
        public Operation? Operation { get; set; }
        public int? OperationId { get; set; }

        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }

        public string? Observer { get; set; }
        public string? Operator { get; set; }

        public int? Option { get; set; }
        public string? Anomaly { get; set; }


        public string? Time1HOE { get; set; }
        public string? Time2HOE { get; set; }
        public string? Models { get; set; }
        public string? Cicles { get; set; }

        public string? SArea { get; set; }
        public string? QArea { get; set; }
        public string? DArea { get; set; }
        public string? CArea { get; set; }
        public string? OthersArea { get; set; }

        public string? IdentifiedActivity { get; set; }
        public string? SsvCommentary { get; set; }
        public string? OperatorCommentary { get; set; }
        public string? SsvSignature { get; set; }
        public string? OperatorSignature { get; set; }

    }
}
