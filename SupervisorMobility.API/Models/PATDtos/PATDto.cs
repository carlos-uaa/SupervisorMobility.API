using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.ILURegisterDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.PATDtos
{
    public class PATDto
    {

        public int PATid { get; set; }
        public int Status { get; set; }
        public int SupervisorId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails? Supervisor { get; set; }
        public int? SSVresponsibleID { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails? SSVresponsible { get; set; }

        public int PlantId { get; set; }
        public PlantDto? Plant { get; set; }
        public int AreaId { get; set; }
        public AreaDtos.AreaWithoutNavigationPropertiesDto? Area { get; set; }
        //public int DistributionId { get; set; }
        //public DistributionDtos.DistributionWithoutNavigationPropertiesDto? Distribution { get; set; }
        public ICollection<LeadershipRecordsDto>? LeadershipRecords { get; set; }
        public ICollection<PatUserRole>? PatUserRoles { get; set; }

        public int? KnowledgePercentage { get; set; }


        public DateTime? AplicationDate { get; set; }
        public int? AplicationYear { get; set; }
     
        public DateTime? CreationDate { get; set; }

        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public int? SOSHubId { get; set; }
        public SOSHubDto? SOSHub { get; set; }

        public string? HistoricalAbility { get; set; }

    }
}
