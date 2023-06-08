using SupervisorMobility.API.Models.LogSpecificEventDtos;

namespace SupervisorMobility.API.Models.LogEventDtos
{
    public class LogEventForUpdateDto
    {
        public string EventDescription { get; set; }
        public ICollection<LogSpecificEventDto>? SpecificEvents { get; set; }
    }
}
