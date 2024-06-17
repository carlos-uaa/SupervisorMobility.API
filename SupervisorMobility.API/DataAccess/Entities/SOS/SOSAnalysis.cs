using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSAnalysis
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSAnalysisId { get; set; }






        public bool? IsActive { get; set; }

        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
    }
}
