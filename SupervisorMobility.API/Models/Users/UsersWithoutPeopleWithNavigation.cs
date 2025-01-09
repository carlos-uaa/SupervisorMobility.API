using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.DepartmentDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.GroupDtos;
using SupervisorMobility.API.Models.ILURegisterDtos;
using SupervisorMobility.API.Models.PlantDtos;

namespace SupervisorMobility.API.Models.Users
{
    public class UsersWithoutPeopleWithNavigation
    {
        public int UserId { get; set; }
        public string? ObjectId { get; set; }
        public int? Payroll { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public int UserType { get; set; }

        public int? SuperiorId { get; set; }
        public string? Management { get; set; }
        public string? Process { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; }
        public DateTime? DisabledDate { get; set; }

        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? GroupId { get; set; }
        public int? DistributionId { get; set; }
        public int? DepartmentId { get; set; }

        public DistributionWithoutNavigationPropertiesDto? Distribution { get; set; }
        public PlantDto? Plant { get; set; } = new PlantDto();
        public AreaWithoutNavigationPropertiesDto? Area { get; set; } = new AreaWithoutNavigationPropertiesDto();
        public ICollection<AreaDtos.AreaWithoutNavigationPropertiesDto>? Areas { get; set; }
        public GroupDto? Group { get; set; } = new GroupDto();
        public DepartmentDto? Department { get; set; } = new DepartmentDto();

        public ICollection<ILURegisterWithoutNavigationDto>? ILURegisers { get; set; }


        public DateTime? IncomesDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? ProfilePictureId { get; set; }

        public FileUploadGeneralDto? ProfilePicture { get; set; }

        public int? HciId { get; set; }

    }
}
