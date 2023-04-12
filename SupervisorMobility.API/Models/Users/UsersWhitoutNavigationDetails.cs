using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.GroupDtos;
using SupervisorMobility.API.Models.PlantDtos;

namespace SupervisorMobility.API.Models.Users
{
    public class UsersWhitoutNavigationDetails
    {
        public int UserId { get; set; }
        public string? ObjectId { get; set; }
        public int? Payroll { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool? IsAdmin { get; set; }
        public bool? IsSupervisor { get; set; }
        public bool? IsOperator { get; set; }
        public bool? IsActive { get; set; }
        public int PlantId { get; set; }
        public int AreaId { get; set; }
        public int GroupId { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; }
        public DateTime? DisabledDate { get; set; }
    }
}
