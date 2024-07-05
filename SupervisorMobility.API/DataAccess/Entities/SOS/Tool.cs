using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class Tool
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ToolId { get; set; }
        public string ToolCode { get; set; }
        public string ToolName { get; set; }
        public bool? IsActive { get; set; }

        public ICollection<SOSHub>? ToolsUsed { get; set; } = new List<SOSHub>();
    }
}
