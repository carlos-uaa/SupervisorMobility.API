using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class ProductDistribution
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductDistributionId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        //Navigation properties
        public int ProductId { get; set; }

        public ICollection<Operation> Operations { get; set; }
            = new List<Operation>();

        public ProductDistribution(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
