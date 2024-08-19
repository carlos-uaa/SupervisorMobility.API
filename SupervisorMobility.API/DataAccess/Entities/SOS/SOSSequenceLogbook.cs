using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.DataAccess.Entities.IS;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSSequenceLogbook
    {
     
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSSequenceLogbookId { get; set; }
        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int SOSSequenceId { get; set; }
        public SOSSequence? SOSSequence { get; set; }


        public int? ApproverId { get; set; }
        public User? Approver { get; set; }
        public FileUpload? ApproverSignatureImage { get; set; } = new();

        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public FileUpload? ReviewerSignatureImage { get; set; } = new();

    }
}
