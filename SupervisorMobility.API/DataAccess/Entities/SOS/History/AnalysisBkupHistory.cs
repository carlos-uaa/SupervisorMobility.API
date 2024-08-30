using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class AnalysisBkupHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnalysisBkupHistoryId { get; set; }
        public int AnalysisBkupId { get; set; }
        public string Text { get; set; }
        public bool? IsActive { get; set; }
        public ICollection<SOSHubHistory>? AnalysisBkups { get; set; } = new List<SOSHubHistory>();

    }
}
