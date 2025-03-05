using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.ILURegisterDtos;
using SupervisorMobility.API.Models.PATDtos.PatDistributionCommentDtos;
using SupervisorMobility.API.Models.PATDtos.PatSubordinateDtos;
using SupervisorMobility.API.Models.PATDtos.PatUserRoleDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.PATDtos
{
    public class PATForUpdateDto
    {
        public int Status { get; set; }


        public int PlantId { get; set; }
        public int AreaId { get; set; }

        public int? KnowledgePercentage { get; set; }


        public DateTime? AplicationDate { get; set; }
        public int? AplicationYear { get; set; }


        public DateTime? CreationDate { get; set; }

        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public int? SOSHubId { get; set; }
        public string? HistoricalAbility { get; set; }
        public string? SaveLeader { get; set; }

        public ICollection<UsersWithoutPeopleWithNavigation>? Supervisors { get; set; }
        public ICollection<PatSubordinateForUpdateDto>? PatSubordinates { get; set; }
        public ICollection<PatUserRoleForUpdateDto>? PatUserRoles { get; set; }
        public ICollection<PatDistributionCommentForUpdateDto>? PatDistributionComments { get; set; }

    }
}
