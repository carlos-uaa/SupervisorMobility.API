using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.Logger
{
    public class Logger
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public string? EventPhase { get; set; }

        public int EventId { get; set; }
        public LogEvent Event { get; set; }

        public int SpecificEventId { get; set; }
        public LogSpecificEvent SpecificEvent { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? DateOfEvent { get; set; } = DateTime.Now;

        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public string? ExceptionMsg { get; set; }
    }
}
