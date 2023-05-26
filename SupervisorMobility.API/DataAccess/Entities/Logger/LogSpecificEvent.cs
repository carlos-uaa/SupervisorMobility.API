using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.Logger
{
    public class LogSpecificEvent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogSpecificEventId { get; set; }
        public string SpecificEventDescription { get; set; }
    }
}
