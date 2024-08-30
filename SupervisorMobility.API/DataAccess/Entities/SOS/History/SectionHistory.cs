using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SectionHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SectionHistoryId { get; set; }
        public int SectionId { get; set; }
        public ICollection<AnalysisHistory> Analyses { get; set; } = new List<AnalysisHistory>();
        public string Time { get; set; } = "";
        public string Step { get; set; } = "";
        public bool? IsActive { get; set; }

    }
}
