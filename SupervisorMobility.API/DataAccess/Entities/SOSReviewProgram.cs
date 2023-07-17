using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class SOSReviewProgram
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSid { get; set; }
        public int Status { get; set; }

        public int? UserAid { get; set; }

        private User? UserA;
        public int? UserBid { get; set; }

        private User? UserB;
        public int? UserCid { get; set; }

        private User? UserC;


        public int? PlantId { get; set; }
        [ForeignKey("PlantId")]
        [NotMapped]
        public Plant? Plant { get; set; }

        public int? AreaId { get; set; }
        [ForeignKey("AreaId")]
        [NotMapped]

        public Area? Area { get; set; }



        [Column(TypeName = "Date")]
        public DateTime? CreationDate { get; set; }
        public int? AplicationYear
        {
            get { return CreationDate?.Year; }
            set { CreationDate = value != null ? new DateTime(value.Value, 1, 1) : null; }
        }


        [Column(TypeName = "Date")]
        public DateTime? EditionDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? ApprovalDate { get; set; }

        public bool IsActive { get; set; }
    }
}
