using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.TurnDtos
{
    public class TurnDto
    {
        public int TurnId { get; set; }

        public string? TurnType { get; set; }

        public int? OperatorId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails? Operator { get; set; }

        public int? SupervisorId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails? Supervisor { get; set; }
    }
}
