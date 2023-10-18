using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.DataAccess.Entities.SOS_Review
{
    public class SOSRegUserOperation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSRegUserOperationId { get; set; }

        public int? SOSReviewProgramid { get; set; }
        public SOSReviewProgram? SOSReviewProgram { get; set; }

        public int? OperationId { get; set; }
        public Operation? Operation { get; set; } 
        
        public int? SupervisorId { get; set; }
        public User? Supervisor { get; set; }
    }
}
