
// - Core .NET imports
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// - Custom project imports
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO.Collections;

namespace SupervisorMobility.API.DataAccess.Entities.SOS.STRO
{
    public class OperationMachine
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Operation { get; set; } = string.Empty;
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public int SectionId { get; set; }
    }
}