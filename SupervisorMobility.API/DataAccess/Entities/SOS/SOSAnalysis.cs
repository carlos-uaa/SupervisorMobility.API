using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Org.BouncyCastle.Asn1;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSAnalysis
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSAnalysisId { get; set; }

        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }
        public string? InternalControlNumber {  get; set; } //Folio

        public ICollection<SOSAnalysisLogbook>? AnalysisLogbooks { get; set; } = new List<SOSAnalysisLogbook>();

        public ICollection<FileUpload>? Illustrations { get; set; } = new List<FileUpload>();
        public ICollection<Commentary>? Notes { get; set; } = new List<Commentary>();
        public ICollection<SOSTime>? Times { get; set; } = new List<SOSTime>();
     

        [Column(TypeName = "Date")]
        public DateTime? CreatedDate { get; set; }

        public bool? IsActive { get; set; }

        public int? SOSHubId { get; set; }
        [ForeignKey("SOSHubId")]
        public SOSHub? SOSHub { get; set; }
    }
}
