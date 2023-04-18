using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.DistributionDtos;

namespace SupervisorMobility.API.Models.AttendanceDtos
{
    public class AttendanceWithNavigationDetailsDto
    {
        public int AttendanceId { get; set; }

        public int? SuperiorId { get; set; }
        public int? UserId { get; set; }
        public int? CurrentdistributionId { get; set; }

        public UsersWithoutNavigationDetails? User { get; set; } = new UsersWithoutNavigationDetails();
        public UsersWithoutNavigationDetails? Superior { get; set; } = new UsersWithoutNavigationDetails();
        public DistributionWithoutNavigationPropertiesDto? Currentdistribution { get; set; } = new DistributionWithoutNavigationPropertiesDto();

        
        public bool Compas { get; set; }
        public bool Station { get; set; }
    }
}
