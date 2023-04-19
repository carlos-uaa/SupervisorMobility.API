using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.DataAccess.Entities;


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
        public bool? IsActive { get; set; }

        //Navigation properties
        public int DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public Operation(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
