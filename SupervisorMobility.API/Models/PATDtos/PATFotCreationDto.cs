using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.PATDtos
{
    public class PATFotCreationDto
    {
        public int SupervisorId { get; set; }

        public int? SSVresponsibleID { get; set; }

        public int AreaId { get; set; }

        public int DistributionId { get; set; }


        public DateTime? AplicationDate { get; set; }
        public int? AplicationYear
        {
            get { return AplicationDate?.Year; }
            set { AplicationDate = value != null ? new DateTime(value.Value, 1, 1) : null; }
        }

        public DateTime? CreationDate { get; set; }

        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }
    }
}
