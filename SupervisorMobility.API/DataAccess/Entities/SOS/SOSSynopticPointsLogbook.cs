using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSSynopticPointsLogbook
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSSynopticPointsLogbookId { get; set; }
        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int SOSSynopticTableofControlPointsId { get; set; }
        public SOSSynopticTableofControlPoints? SOSSynopticTableofControlPoints { get; set; }

        public int? ApproverId { get; set; }
        public User? Approver { get; set; }
        public FileUpload? ApproverSignatureImage { get; set; } = new();

    }
}