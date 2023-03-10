using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Entities
{
    public class Lup
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LupId { get; set; }
        public int JobObservationId { get; set; }

        public string? Oportunity { get; set; }

        public bool? IsActive { get; set; }

        public string? Observer { get; set; }
        public int Pillar { get; set; }
        public string? Q3 { get; set; }
        public string? Q4 { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; set;}

        //Evidence
        //public ICollection<FileUpload> Evidences { get; set; }
        //    = new List<FileUpload>();
    }
}
