using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class Area
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AreaId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        //Navigation properties
        public int PlantId { get; set; }
        public Plant? Plant { get; set; }

        public ICollection<Distribution> Distributions { get; set; }
            = new List<Distribution>();
        [NotMapped]
        public ICollection<User>? Users { get; set; }

        public Area(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
