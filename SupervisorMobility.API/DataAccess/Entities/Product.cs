using SupervisorMobility.API.DataAccess.Entities.SOS;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Distribution> Distributions { get; set; }
            = new List<Distribution>();
        
        public virtual ICollection<SOSHub> SOSHubs { get; set; }
            = new List<SOSHub>();

        public Product(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
