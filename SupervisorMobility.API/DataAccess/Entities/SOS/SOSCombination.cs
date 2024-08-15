using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSCombination
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSCombinationId { get; set; }
        public bool? IsActive { get; set; }

        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public FileUpload? ReviewerSignatureImage { get; set; } = new();

        public int? ReviewerHSId { get; set; }
        public User? ReviewerHS { get; set; }
        public FileUpload? ReviewerHSSignatureImage { get; set; } = new();

        public int? ApproverId { get; set; }
        public User? Approver { get; set; }
        public FileUpload? ApproverSignatureImage { get; set; } = new();

        public ICollection<Turn>? Turns { get; set; }

        public DateTime? Date { get; set; }

        public string? ProductionVolumePerShift { get; set; }
        public string? ControlNumber { get; set; }


        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
    }
}
