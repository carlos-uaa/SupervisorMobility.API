using DocumentFormat.OpenXml.Spreadsheet;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationID { get; set; }

        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        public string? MadeBy { get; set; }

        public int TargetRelation { get; set; }
        public string NotificationType { get; set; }

        public Users? User { get; set; }

        public bool IsAccepted { get; set; } = false;
        public DateTime EntryDate { get; set; } = DateTime.Now;
    }
}
