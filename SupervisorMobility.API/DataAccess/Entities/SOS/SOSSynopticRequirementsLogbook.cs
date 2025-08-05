using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSSynopticRequirementsLogbook
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSSynopticRequirementsLogbookId { get; set; }
        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int SOSSynopticRequirementsId { get; set; }
        public SOSSynopticTableofOperatingRequirements? SOSSynopticRequirements { get; set; }


        public int? ApproverId { get; set; }
        public User? Approver { get; set; }
        public FileUpload? ApproverSignatureImage { get; set; } = new();

        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public FileUpload? ReviewerSignatureImage { get; set; } = new();

    }
}