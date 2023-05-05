using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class Plant
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlantId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        //Navigation Property
        [NotMapped]
        public ICollection<Area> Areas { get; set; }
            = new List<Area>();

        public Plant(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
