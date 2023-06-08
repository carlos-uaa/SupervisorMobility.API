using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class Notification
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationID { get; set; }

        public string? MadeBy { get; set; }

        //public int TargetRelation { get; set; }
        public string NotificationType { get; set; }
        public string NotificationText { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public bool IsAccepted { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public DateTime EntryDate { get; set; } = DateTime.Now;
    }
}
