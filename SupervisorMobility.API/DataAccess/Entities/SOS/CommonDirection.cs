using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class CommonDirection
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommonDirectionId { get; set; }
        public int DOC_ID { get; set; }
        public string route { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public int type { get; set; } //To check origin of file (CCP/GOS)
        public bool IsActive { get; set; } = true;
    }
}
