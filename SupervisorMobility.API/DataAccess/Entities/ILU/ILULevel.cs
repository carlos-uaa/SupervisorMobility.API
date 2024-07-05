using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.LUP
{
    public class ILULevel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ILULevelId { get; set; }

        public string ILULevelCode { get; set; } = string.Empty;
        public string ILULevelDescription { get; set; } = string.Empty;

        public bool isActive { get; set; }

    }
}
