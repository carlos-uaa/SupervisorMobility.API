using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class PAT
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PATid { get; set; }

        public int? SupervisorId { get; set; }
        public int Status { get; set; }

        public User? Supervisor
        {
            get { return _supervisor; }
            set
            {
                _supervisor = value;
                if (_supervisor != null)
                {
                    SSVresponsibleID = _supervisor.SuperiorId;
                    AreaId = (int)_supervisor.AreaId;
                }
                else
                {
                    SSVresponsibleID = null;
                }
            }
        }
        private User? _supervisor;

        public int? SSVresponsibleID { get; set; }

        public User? SSVresponsible { get; set; }

        public int? PlantId { get; set; }
        [ForeignKey("PlantId")]
        [NotMapped]
        public Plant? Plant { get; set; }

        public int? AreaId { get; set; }
        [ForeignKey("AreaId")]
        [NotMapped]

        public Area? Area { get; set; }

        //public int? DistributionId { get; set; }
        //[ForeignKey("DistributionId")]
        //[NotMapped]
        //public Distribution? Distribution { get; set; }



        [Column(TypeName = "Date")]
        public DateTime? AplicationDate { get; set; }
        public int? AplicationYear
        {
            get { return AplicationDate?.Year; }
            set { AplicationDate = value != null ? new DateTime(value.Value, 1, 1) : null; }
        }


        [Column(TypeName = "Date")]
        public DateTime? CreationDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? EditionDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? ApprovalDate { get; set; }

        public bool IsActive { get; set; }
    }
}
