using SupervisorMobility.API.Models.JobObservationDtos;

namespace SupervisorMobility.API.Models.ADUser
{
    public class RequestJobObservationADuser
    {
        public JobObservationForUpdateDto JobObservation { get; set; }
        public string LoggedUser { get; set; }
    }
}
