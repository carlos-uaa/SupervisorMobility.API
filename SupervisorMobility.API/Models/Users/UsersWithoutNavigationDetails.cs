using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.GroupDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.Users
{
    public class UsersWithoutNavigationDetails
    {
        public int UserId { get; set; }
        public string? ObjectId { get; set; }
        public int? Payroll { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; } 
        public int UserType { get; set; }

        public int? SuperiorId { get; set; }

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
    }
}


