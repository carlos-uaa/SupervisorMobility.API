using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class RequestJobObservationADuser
    {
       public JobObservation JobObservation { get; set; }
       public ADuser ADuser { get; set; }  
    }
}
