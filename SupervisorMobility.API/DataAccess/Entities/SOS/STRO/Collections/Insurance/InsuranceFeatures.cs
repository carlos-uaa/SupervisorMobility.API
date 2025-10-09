// - Core .NET imports
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// - Custom project imports
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO;

namespace SupervisorMobility.API.DataAccess.Entities.SOS.STRO
{
    public class InsuranceFeatures
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Insurance { get; set; } = string.Empty;
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public int SectionId { get; set; }
    }
}