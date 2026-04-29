using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities
{
    public class HRIHistoryActions
    {
        [Key]
        public int HistoryId { get; set; }
        public int HRIid { get; set; }
        public int? ResponsibleUserId { get; set; }
        public User? Responsible { get; set; }
        public string? Action { get; set; }
        public DateTime? ActionDate { get; set; }
    }
}
