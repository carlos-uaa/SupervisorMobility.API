using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;
using System.Text.Json.Serialization;

namespace SupervisorMobility.API.DataAccess.Entities.LUP
{
    public class ILULevel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ILULevelId { get; set; }

        public char ILULevelCode { get; set; }
        public string ILULevelDescription { get; set; } = string.Empty;
        
        public bool isActive { get; set; }

    }
}
