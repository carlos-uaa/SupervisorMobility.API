using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.Logger
{
    public class Event
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }
        public string EventDescription { get; set; }
        public ICollection<SpecificEvent>? SpecificEvents { get; set; }
    }
}
