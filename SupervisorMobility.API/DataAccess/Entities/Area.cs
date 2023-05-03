using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;

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

        public ICollection<User> Users { get; set; }  = new List<User>();

        public Area(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
