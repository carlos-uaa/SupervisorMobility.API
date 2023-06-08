using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.PATDtos
{
    public class PATDto
    {

        public int PATid { get; set; }

        public int SupervisorId { get; set; }
        public UsersWithoutNavigationDetails? Supervisor { get; set; }
        public int? SSVresponsibleID { get; set; }
        public UsersWithoutNavigationDetails? SSVresponsible { get; set; }

        public int PlantId { get; set; }
        public PlantDto? Plant { get; set; }
        public int AreaId { get; set; }
        public AreaDtos.AreaWithoutNavigationPropertiesDto? Area { get; set; }
        public int DistributionId { get; set; }
        public DistributionDtos.DistributionWithoutNavigationPropertiesDto? Distribution { get; set; }

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
