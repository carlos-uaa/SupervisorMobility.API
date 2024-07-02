using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class AnalysisBkup
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnalysisBkupId { get; set; }

        public string Text { get; set; }
        public bool? IsActive { get; set; }

    }
}
