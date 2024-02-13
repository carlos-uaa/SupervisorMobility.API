using DocumentFormat.OpenXml.Bibliography;
using SupervisorMobility.API.DataAccess.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class Lup
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LupId { get; set; }
        public int JobObservationId { get; set; }
        public JobObservation JobObservation {  get; set; }
        public List<Findings> Findings { get; set; } = new List<Findings>();

        public string? Oportunity { get; set; }

        public bool? IsActive { get; set; }

        public string? Observer { get; set; }
        public int Pillar { get; set; }
        public string? Q3 { get; set; }
        public string? Q4 { get; set; }
        public string? Justification { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<FileUpload>? Evidences { get; set; }
            = new List<FileUpload>();

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }


    }

    public class Findings
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FindingId { get; set; }
        public string Valor { get; set; }
        public int LupId { get; set; } // Clave foránea
    }
}
