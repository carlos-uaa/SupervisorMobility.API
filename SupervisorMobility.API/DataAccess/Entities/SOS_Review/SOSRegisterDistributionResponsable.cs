using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS_Review
{
    public class SOSRegisterDistributionResponsable
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSRegisterDistributionid { get; set; }

        public int? DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public int? Responsableid { get; set; }
        public User? Responsable { get; set; }  

        public int? SOSReviewProgramid { get; set; }
        public SOSReviewProgram? SOSReviewProgram { get; set; }


    }
}
