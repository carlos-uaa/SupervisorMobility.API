using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities.Logger
{
    public class LogSpecificEvent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogSpecificEventId { get; set; }
        public string SpecificEventDescription { get; set; }
    }
}
