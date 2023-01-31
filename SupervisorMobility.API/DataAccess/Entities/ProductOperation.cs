using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class ProductOperation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductOperationId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        //Navigation properties
        public int ProductDistributionId { get; set; }
        public ProductDistribution? productDistribution { get; set; }

        public ProductOperation(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
