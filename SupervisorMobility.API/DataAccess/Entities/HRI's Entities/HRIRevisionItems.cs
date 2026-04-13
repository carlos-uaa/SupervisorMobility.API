using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionsItem_Entities;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI
{
    public class HRIRevisionItems
    {
        [Key]
        public int ItemId { get; set; }
        public int HriId { get; set; }
        public HRI HRI { get; set; }
        public int ItemNumber { get; set; }
        public string  RevisionPoint { get; set; }
        public int? RevisionMethodId { get; set; } //por confirmar
        public RevisionMethod? RevisionMethod { get; set; }
        public int? VeredictId { get; set; }
        public Veredict? Veredict { get; set; }
        public int? FrequencyId { get; set; }//por confirmar
        public Frequency? Frequency { get; set; }
    }
}
