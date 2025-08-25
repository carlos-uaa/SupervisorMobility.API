using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSSynopticTableofOperatingRequirements
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }

        public string? InternalControlNumber { get; set; }
        public string? ProcessName { get; set; }


        public int? CreatorId { get; set; }
        public User? Creator { get; set; }
        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public int? ApproverId { get; set; }
        public User? Approver { get; set; }


        public DateTime? CreatedAt { get; set; }


        public ICollection<SOSSynopticRequirementsOperationSequence>? SOSSynopticRequirementsOperationSequence { get; set; }

        public ICollection<SOSSynopticRequirementsLogbook>? SynopticRequirementsLogbooks { get; set; } = new List<SOSSynopticRequirementsLogbook>();


        public bool? IsActive { get; set; }
        public int? SOSHubId { get; set; }

        public ICollection<SOSHub>? SOSHubs { get; set; } = new List<SOSHub>();
        //las analisis y las secuencias de las que se sacaran los puntos principal
        public ICollection<SOSAnalysis>? Analyses { get; set; } = new List<SOSAnalysis>();
        public ICollection<SOSSequence>? Sequences { get; set; } = new List<SOSSequence>();
    }
}