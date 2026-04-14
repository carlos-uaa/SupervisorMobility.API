using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities
{
    public class RevisionCycles
    {
        [Key]
        public int RevisionCycleId { get; set; }
        public int Cycle { get; set; }
        public int? HRIRevisionItemsId { get; set; }
        public HRIRevisionItems? HRIRevisionItems { get; set; }       
        public List<DailyRevisions>? DailyRevisions { get; set; }

    }
}
