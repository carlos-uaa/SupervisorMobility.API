using SupervisorMobility.API.DataAccess.Entities.Logger;
using SupervisorMobility.API.Models.LogSpecificEventDtos;

namespace SupervisorMobility.API.Models.LogEventDtos
{
    public class LogEventForCreateDto
    {

        public string EventDescription { get; set; }
        public ICollection<LogSpecificEventDto>? SpecificEvents { get; set; }
    }
}
