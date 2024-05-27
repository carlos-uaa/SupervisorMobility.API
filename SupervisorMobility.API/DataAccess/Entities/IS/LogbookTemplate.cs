using SupervisorMobility.API.DataAccess.Entities.Paths;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.IS
{
    public class LogbookTemplate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogbookTemplateId { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int? TemplateId { get; set; }
        public Template? TemplateInspection {  get; set; }


        [Column(TypeName = "Date")]
        public DateTime? Date { get; set; }

        [Column(TypeName = "Time")]
        public TimeSpan? Time { get; set; }
        public string? RAN { get; set; }

        public ICollection<CheckpointNormAnswer>? CheckpointsResults { get; set; }
= new List<CheckpointNormAnswer>();

        public int? InspectorId { get; set; }
        public User? Inspector { get; set; }
        public string? InspectorSignature { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? InspectorSignatureDate { get; set; }
        public ICollection<Commentary>? InspectorObservations { get; set; }
        public int? SupervisorId { get; set; }
        public User? Supervisor { get; set; }
        public string? SupervisorSignature { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? SupervisorSignatureDate { get; set; }

      
    }
}
