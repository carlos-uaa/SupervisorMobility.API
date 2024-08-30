using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class AnalysisHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnalysisHistoryId { get; set; }
        public int AnalysisId { get; set; }
        public string? Text { get; set; }
        public List<string>? CriticalPoints { get; set; } = new List<string>();
        public List<string>? Reasons { get; set; } = new List<string>();
        public bool? IsActive { get; set; }
    }
}
