using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.ILURegisterDtos;

namespace SupervisorMobility.API.Models.Users
{
    public class UsersForUpdateDto
    {
        public string? ObjectId { get; set; }
        public int? Payroll { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;

        public int UserType { get; set; }
        public int? SuperiorId { get; set; }

        public string? Management { get; set; }
        public string? Department { get; set; }
        public string? Process { get; set; }


        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public DateTime? DisabledDate { get; set; }

        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? GroupId { get; set; }
        public int? DistributionId { get; set; }
        public int? DepartmentId { get; set; }

        public ICollection<UsersWithoutNavigationWithoutPeopleDetails>? Subordinates { get; set; }
        //public ICollection<ILURegisterWithoutNavigationDto>? ILURegisers { get; set; }

        public ICollection<AreaDtos.AreaWithoutNavigationPropertiesDto>? Areas { get; set; }

        public DateTime? IncomesDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? ProfilePictureId { get; set; }
        public int? HciId { get; set; }

        public FileUploadGeneralDto? ProfilePicture { get; set; }
    }
}
