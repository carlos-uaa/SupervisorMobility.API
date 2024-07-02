using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class Analysis
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnalysisId { get; set; }
        public string Text { get; set; }
        public string CriticalPoint { get; set; } = string.Empty;
        public string Reason { get; set; }
        public bool? IsActive { get; set; }

    }
}
