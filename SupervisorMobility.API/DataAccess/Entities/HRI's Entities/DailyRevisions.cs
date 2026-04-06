using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI
{
    public class DailyRevisions
    {
        [Key]
        public int RevisionId { get; set; }
        public int? CycleId { get; set; }
        public HRICycles? cycle { get; set; }
        public int? RevisionCycleId { get; set; }
        public RevisionCycles? RevisionCycle { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
    }
}
