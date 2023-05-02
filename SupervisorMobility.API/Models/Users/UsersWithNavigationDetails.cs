using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.GroupDtos;
using SupervisorMobility.API.Models.PlantDtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.Users
{
    public class UsersWithNavigationDetails
    {
        public int UserId { get; set; }
        public string? ObjectId { get; set; }
        public int? Payroll { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public int UserType { get; set; }

        public int? SuperiorId { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; }
        public DateTime? DisabledDate { get; set; }

        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? GroupId { get; set; }
        public int? DistributionId { get; set; }


        public DistributionWithoutNavigationPropertiesDto? Distribution { get; set; }
        public PlantDto? Plant { get; set; } = new PlantDto();
        public AreaWithoutNavigationPropertiesDto? Area { get; set; } = new AreaWithoutNavigationPropertiesDto();
        public GroupDto? Group { get; set; } = new GroupDto();


        public UsersWithoutNavigationDetails? Superior { get; set; }
        public ICollection<UsersWithoutNavigationDetails>? Subordinates { get; set; }
        public ICollection<AreaDtos.AreaWithoutNavigationPropertiesDto>? Areas { get; set; }

    }
}
