using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities
{
    public class WeeklyRevisions
    {
        [Key]
        public int RevisionId  { get; set; }
        public int HriId { get; set; }
        public HRI HRI { get; set; }
        public int? UserId { get; set; }
        public User? SeniorSupervisor { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
        public int Year { get; set; }
        public bool? IsActive { get; set; }
        public string? Status { get; set; }
    }
}
