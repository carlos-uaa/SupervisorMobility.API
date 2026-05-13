using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities
{
    public class HRICycles
    {
        [Key]
        public int CycleId { get; set; }
        public int Cycle { get; set; }
        public int HriId { get; set; }
        public HRI HRI { get; set; }
        public int?  SupervisorUserId { get; set; }
        public User? Supervisor { get; set; }
        public int?  OperatorUserId { get; set; }
        public User? Operator { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }
        public List<DailyRevisions>? DailyRevisions { get; set; }
    }
}
