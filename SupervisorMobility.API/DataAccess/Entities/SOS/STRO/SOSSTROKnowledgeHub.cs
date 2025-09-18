// - Core .NET imports
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// - Custom project imports
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSSTROKnowledgeHub
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int KnowledgeId { get; set; }
        public Knowledge? Knowledge { get; set; }
        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public SOSSynopticTableofOperatingRequirements? SOSSynopticTableofOperatingRequirements{ get; set; }
    }
}