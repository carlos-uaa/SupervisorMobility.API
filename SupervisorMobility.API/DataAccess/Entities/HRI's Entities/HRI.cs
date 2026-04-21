using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities
{
    public class HRI
    {
        [Key]
        public int HriId { get; set; }
        public int? HRILinesId { get; set; }
        public HRILines? Line { get; set; }
        public int? HRIItemId { get; set; }
        public HRIItem? NameOfItem { get; set; }
        public string? ControlNumber { get; set; }
        public int? HRIDockId { get; set; }
        public HRIDock? Dock { get; set; }
        public string? Department { get; set; }
        public List<HRImages>? Images { get; set; }
        public List<HRIRevisionItems>? ItemsRevised { get; set; }
        public List<WeeklyRevisions>? WeeklyRevisions { get; set; }
        public List<HRICycles>? HriCycles { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public HourmeterRevision? HourmeterRevision { get; set; }
        public int? SupervisorUserId { get; set; }
        public User? Supervisor { get; set; }
         public int? SSVUserId { get; set; }
        public User? SSV { get; set; }
        public  int? PlantId { get; set; }
        public Plant? Plant { get; set; }
        public int? AreaId { get; set; }
        public Area? Area { get; set; }
    }
}
