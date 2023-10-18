using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.Logger
{
    public class LogEvent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogEventId { get; set; }
        public string EventDescription { get; set; }
        public ICollection<LogSpecificEvent>? SpecificEvents { get; set; }
    }
}
