using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI
{
    public class HRI
    {
        [Key]
        public int HriId { get; set; }
        public HRILines? Line { get; set; }
        public HRIItem? NameOfItem { get; set; }
        public string? ControlNumber { get; set; }
        public HRIDock? Dock { get; set; }
        public string? Department { get; set; }
        public List<HRImages>? Images { get; set; }
        public List<HRIRevisionItems>? ItemsRevised { get; set; }
        public List<WeeklyRevisions>? WeeklyRevisions { get; set; }
        public List<HRICycles>? HriCycles { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DailyRevisions? HourmeterRevision { get; set; }
    }
}
