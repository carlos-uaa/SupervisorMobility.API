using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSTime
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSTimeId { get; set; }
        public int SectionId { get; set; }
        public int? AnalysisId { get; set; }
        public string? Time { get; set; } = "";
    
        public bool? IsActive { get; set; }
    }
}
