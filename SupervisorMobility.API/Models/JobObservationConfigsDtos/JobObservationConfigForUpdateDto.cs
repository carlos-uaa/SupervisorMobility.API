using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.JobObservationConfigsDtos
{
    public class JobObservationConfigForUpdateDto
    {
        [Required]
        public int JobObservationTypeId { get; set; }
        [Required]
        public int ChecklistCategoryId { get; set; }
    }
}
