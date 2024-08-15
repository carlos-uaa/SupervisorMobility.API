using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Kiota.Abstractions;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSFlowLogbook
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSFlowLogbookId { get; set; }

        public int SOSFlowId { get; set; }
        public SOSFlow? SOSFlow { get; set; }

        public int? ApproverId { get; set; }
        public User? Approver { get; set; }
        public FileUpload? ApproverSignatureImage { get; set; } = new();

        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public bool? IsActive { get; set; }

    }
}