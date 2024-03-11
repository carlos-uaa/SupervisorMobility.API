using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class Pillar
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PillarId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        public ICollection<ChecklistQuestion>? ChecklistQuestions { get; set; } = new List<ChecklistQuestion>();


        public Pillar(string code, string description)
        {
            Code = code;
            Description = description;
        }


    }
}
