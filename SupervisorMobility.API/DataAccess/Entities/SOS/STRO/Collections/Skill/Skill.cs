// - Core .NET imports
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// - Custom project imports
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO.Collections.Skill;

namespace SupervisorMobility.API.DataAccess.Entities.SOS.STRO
{
    public class Skill
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
