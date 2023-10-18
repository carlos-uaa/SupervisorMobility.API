using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.DataAccess.Entities.SOS_Review
{
    public class SOSRegisterJobObservation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSRegisterJobid { get; set; }
        public int? SOSReviewProgramid { get; set; }
        public SOSReviewProgram? SOSReviewProgram { get; set; }

        public int? JobObservationId { get; set; }
        public JobObservation? JobObservation { get; set; }

        public int? OperationId { get; set; }
        public Operation? Operation { get; set; }

        public string? Commentary { get; set; }
  
        [Column(TypeName = "DateTime")]
        public DateTime? PreviewDate { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? ConfirmationDate { get; set; }


        [Column(TypeName = "Date")]
        public DateTime? CreationDate { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
