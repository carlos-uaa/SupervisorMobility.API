using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.GroupDtos;
using SupervisorMobility.API.Models.PlantDtos;

namespace SupervisorMobility.API.Models.Users
{
    public class UsersWhitNavigationDetails
    {
        public int UserId { get; set; }
        public int Payroll { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Permissions { get; set; }

        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? GroupId { get; set; }

        public PlantDto? Plant { get; set; } = new PlantDto();
        public AreaWithoutNavigationPropertiesDto? Area { get; set; } = new AreaWithoutNavigationPropertiesDto();
        public GroupDto? Group { get; set; } = new GroupDto();


        public DateTime? CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public DateTime? DisabledDate { get; set; }

    }
}
