using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class Distribution
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DistributionId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        //Navigation properties
        public int AreaId { get; set; }
        public Area? Area { get; set; }

        public ICollection<Operation> Operations { get; set; }
            = new List<Operation>();

        //Navigation Property
        public virtual ICollection<Product> Products { get; set; }
            = new List<Product>();

        public Distribution(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
