using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class Glosary
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GlosaryWordId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }

    }
}
