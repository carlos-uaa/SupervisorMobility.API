using DocumentFormat.OpenXml.Bibliography;
using SupervisorMobility.API.DataAccess.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public enum LUPStatus
{
    Titular,
    Checklist,
    Timer,
    LUP,
    LUP_SSV,
    Signature
}

namespace SupervisorMobility.API.Entities
{
    public class Lup
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LupId { get; set; }
        public int JobObservationId { get; set; }
        public JobObservation JobObservation {  get; set; }

        public string? Oportunity { get; set; }
        public string? Observer { get; set; }
        public int Pillar { get; set; }
        public string? Q3 { get; set; }
        public string? Q4 { get; set; }
        public string? Justification { get; set; }
        public int? Status { get; set; }
        public LUPStatus? StatusOKNG { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<FileUpload>? Evidences { get; set; }
           = new List<FileUpload>();

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? StdChange { get; set; }
        public int? StdUpdate { get; set; }


        public bool? IsActive { get; set; }
    }

}
