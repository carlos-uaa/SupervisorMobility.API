using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SupervisorMobility.API.Entities
{
    public class Operation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OperationId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }


        public string? restrictionorcomment { get; set; }
        public string? jsonTimeProduct { get; set; }

        public string? ProductName { get; set; }
        public string? NameTime { get; set; }
        public string? Time { get; set; } = "§§§§";
        public string? AdditionalTime { get; set; } = "§§§§";
        public string? StandardTime { get; set; } = "§§§§";

        public string? cell { get; set; } = "";

        public int CriticalType { get; set; }

        public bool? IsActive { get; set; }

        //Navigation properties
        public int DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public ICollection<JobObservation> JobObservations { get; set; } = new List<JobObservation>();

        public Operation(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
