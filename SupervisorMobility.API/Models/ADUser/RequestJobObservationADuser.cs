using SupervisorMobility.API.Models.JobObservationDtos;

namespace SupervisorMobility.API.Models.ADUser
{
    public class RequestJobObservationADuser
    {
        public JobObservationForUpdateDto JobObservation { get; set; }
        public ADuser ADuser { get; set; }
    }
}
