using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS_Review
{
    public class SOSReviewProgram
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSid { get; set; }
        public int Status { get; set; }

        public int? Supervisorid { get; set; }
        [ForeignKey("Supervisorid")]
        public User? Supervisor { get; set; }

        public int? PlantId { get; set; }
        [ForeignKey("PlantId")]
        public Plant? Plant { get; set; }

        public int? AreaId { get; set; }
        [ForeignKey("AreaId")]

        public Area? Area { get; set; }



        [Column(TypeName = "Date")]
        public DateTime? CreationDate { get; set; }
        public int? AplicationYear
        {
            get { return CreationDate?.Year; }
            set
            {
                if (value != null)
                {
                    int year = value.Value;
                    if (!(year >= 1 && year <= 9999))
                    {

                        AplicationYear = CreationDate?.Year;
                    }
                }
                else
                {
                    AplicationYear = null;
                }
            }
        }


        [Column(TypeName = "Date")]
        public DateTime? EditionDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? ApprovalDate { get; set; }
        public bool SuggestionApplied { get; set; }

        public bool IsActive { get; set; }
    }
}
