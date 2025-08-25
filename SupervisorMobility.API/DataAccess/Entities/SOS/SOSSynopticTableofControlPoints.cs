using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSSynopticTableofControlPoints
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSSynopticTableofControlPointsId { get; set; }

        public string? InternalControlNumber { get; set; }
        public string? ProcessName { get; set; }


        public int? CreatorId { get; set; }
        public User? Creator { get; set; }
        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public int? ApproverId { get; set; }
        public User? Approver { get; set; }


        public DateTime? CreatedAt { get; set; }

        public ICollection<SOSSynopticPointsOperationSequence>? SOSSynopticPointsOperationSequence { get; set; }


        public ICollection<SOSSynopticPointsLogbook>? SynopticPointsLogbooks { get; set; } = new List<SOSSynopticPointsLogbook>();


        public bool? IsActive { get; set; }
        //Es el id de sos hub que lo creeo y del que se trar la informacion
        public int? SOSHubId { get; set; }

        public IEnumerable<SOSHub>? SOSHubs { get; set; } = new List<SOSHub>();
        //las analisis y las secuencias de las que se sacaran los puntos principal
        public ICollection<SOSAnalysis>? Analyses { get; set; } = new List<SOSAnalysis>();
        public ICollection<SOSSequence>? Sequences { get; set; } = new List<SOSSequence>();

    }
}