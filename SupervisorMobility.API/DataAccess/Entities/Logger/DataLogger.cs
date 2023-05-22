using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.Logger
{
    public class DataLogger
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public string? EventPhase { get; set; }

        public int EventId { get; set; }
        public int SpecificEventId { get; set; }


        [Column(TypeName = "Date")]
        public DateTime? DateOfEvent { get; set; } = DateTime.Now;

        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public string? ExceptionMsg { get; set; }
    }
}
