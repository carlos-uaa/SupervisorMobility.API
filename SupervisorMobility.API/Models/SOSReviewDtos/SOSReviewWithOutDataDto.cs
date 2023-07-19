using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.SOSReviewDtos
{
    public class SOSReviewWithOutDataDto
    {
        public int SOSid { get; set; }
        public int Status { get; set; }

        public int? Supervisorid { get; set; }


        public int? PlantId { get; set; }


        public int? AreaId { get; set; }




        public DateTime? CreationDate { get; set; }
        public int? AplicationYear
        {
            get { return CreationDate?.Year; }
            set { CreationDate = value != null ? new DateTime(value.Value, 1, 1) : null; }
        }


        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public bool IsActive { get; set; }
    }
}
