using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.Models.HRIWeeklyRevisions
{
    public class CreateWeeklyRevisionDto
    {
        public int HriId { get; set; }      
        public int? UserId { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
        public int Year { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; } = true;
        public bool? Notification { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public int? To { get; set; }
        public bool IsUrgent { get; set; }
        public string? CCPEmails { get; set; }
    }
}
