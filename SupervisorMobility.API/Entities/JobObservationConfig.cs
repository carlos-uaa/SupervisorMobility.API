using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class JobObservationConfig
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobObservationConfigId { get; set; }

        //Navigation properties
        public int JobObservationTypeId { get; set; }
        public JobObservationType? JobObservationTypeDto { get; set; }

        public int ChecklistCategoryId { get; set; }
        public ChecklistCategory? ChecklistCategoryDto { get; set; }

    }
}
