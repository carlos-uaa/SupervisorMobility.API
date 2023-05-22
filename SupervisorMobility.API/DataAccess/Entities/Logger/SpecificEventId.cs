using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.Logger
{
    public class SpecificEvent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SpecificEventId { get; set; }
        public string SpecificEventDescription { get; set; }
    }
}
