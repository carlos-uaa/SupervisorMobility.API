using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class Section
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SectionId { get; set; }
        public ICollection<Analysis> Analyses { get; set; } = new List<Analysis>();
        public string Time { get; set; } = "";
        public string Step { get; set; } = "";
        public bool? IsActive { get; set; }

    }
}
