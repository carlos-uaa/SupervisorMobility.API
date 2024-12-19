using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.ILURegisterDtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.Users
{
    public class UsersWithoutNavigationWithoutPeopleDetails
    {
        public int UserId { get; set; }
        public string? ObjectId { get; set; }
        public int? Payroll { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int UserType { get; set; }

        public int? SuperiorId { get; set; }

        public string? Management { get; set; }
        public string? Department { get; set; }
        public string? Process { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        [Column(TypeName = "Date")]
        public DateTime LastUpdated { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? DisabledDate { get; set; }

        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? GroupId { get; set; }
        public int? DistributionId { get; set; }
        public ICollection<ILURegisterWithoutNavigationDto>? ILURegisers { get; set; }
        public DateTime? IncomesDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? ProfilePictureId { get; set; }

        public FileUploadGeneralDto? ProfilePicture { get; set; }

        public int? HciId { get; set; }

    }
}


