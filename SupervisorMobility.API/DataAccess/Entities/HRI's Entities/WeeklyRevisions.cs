using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI
{
    public class WeeklyRevisions
    {
        [Key]
        public int RevisionId  { get; set; }
        public int HriId { get; set; }
        public HRI HRI { get; set; }
        public User SeniorSupervisor { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
    }
}
