using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.AttendanceDtos
{
    public class AttendanceWithNavigationDetailsDto
    {
        public int AttendanceId { get; set; }

        public int? SuperiorId { get; set; }
        public int? UserId { get; set; }
        public int? CurrentdistributionId { get; set; }

        public UsersWhitoutPeopleNavigation? User { get; set; } = new UsersWhitoutPeopleNavigation();
        public UsersWhitoutPeopleNavigation? Superior { get; set; } = new UsersWhitoutPeopleNavigation();
        public DistributionWithoutNavigationPropertiesDto? Currentdistribution { get; set; } = new DistributionWithoutNavigationPropertiesDto();


        public bool Compas { get; set; }
        public bool Station { get; set; }
    }
}
