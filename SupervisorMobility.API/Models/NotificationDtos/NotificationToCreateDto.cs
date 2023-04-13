using SupervisorMobility.API.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.NotificationDtos
{
    public class NotificationToCreateDto
    {
        public string? MadeBy { get; set; }

        //public int TargetRelation { get; set; }
        public string NotificationType { get; set; }
        public string? NotificationText { get; set; }
        public int UserId { get; set; }

        public bool IsAccepted { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public DateTime EntryDate { get; set; } = DateTime.Now;
    }
}
