using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupervisorMobility.API.DataAccess.Entities.SOS.STRO.Enums;
namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSSynopticTableRequirementOperationDifficulty
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public SOSSynopticTableofOperatingRequirements? SOSSynopticTableofOperatingRequirements { get; set; }
        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
    }
}
