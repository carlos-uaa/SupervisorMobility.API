using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.ILURegisterDtos;
using SupervisorMobility.API.Models.PATDtos.PatDistributionCommentDtos;
using SupervisorMobility.API.Models.PATDtos.PatSubordinateDtos;
using SupervisorMobility.API.Models.PATDtos.PatUserRoleDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.PATDtos
{
    public class PATDto
    {

        public int PATid { get; set; }
        public int Status { get; set; }

        public ICollection<UsersWithPeopleWithoutNavigationDetails>? Supervisors { get; set; }
     

        public int PlantId { get; set; }
        public PlantDto? Plant { get; set; }
        public int AreaId { get; set; }
        public AreaDtos.AreaWithoutNavigationPropertiesDto? Area { get; set; }
        public ICollection<PatUserRoleDto>? PatUserRoles { get; set; }
        public ICollection<PatSubordinateDto>? PatSubordinates { get; set; }
        public ICollection<PatDistributionCommentDto>? PatDistributionComments { get; set; }
        public int? KnowledgePercentage { get; set; }


        public DateTime? AplicationDate { get; set; }
        public int? AplicationYear { get; set; }
     
        public DateTime? CreationDate { get; set; }

        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public int? SOSHubId { get; set; }
        public SOSHubDto? SOSHub { get; set; }

        public string? HistoricalAbility { get; set; }
        public string? SaveLeader { get; set; }

    }
}
