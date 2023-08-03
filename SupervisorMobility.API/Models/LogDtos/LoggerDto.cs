using SupervisorMobility.API.Models.LogEventDtos;
using SupervisorMobility.API.Models.LogSpecificEventDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.LOG
{
    public class LoggerDto
    {
        public int LogId { get; set; }

        public int UserId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails? User { get; set; }

        public string? EventPhase { get; set; }

        public int EventId { get; set; }
        public LogEventDto? Event { get; set; }
        public int SpecificEventId { get; set; }
        public LogSpecificEventDto? SpecificEvent { get; set; }

        public DateTime? DateOfEvent { get; set; }

        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public string? ExceptionMsg { get; set; }
    }
}
