using SupervisorMobility.API.Models.SOS.SOSHubDtos;

namespace SupervisorMobility.API.Models.PATDtos
{
    public class PATwithoutNavigations
    {

        public int PATid { get; set; }
        public int Status { get; set; }


        public int PlantId { get; set; }
        public int AreaId { get; set; }

        //public int DistributionId { get; set; }


        public DateTime? AplicationDate { get; set; }
        public int? AplicationYear { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public int? SOSHubId { get; set; }
    }
}
