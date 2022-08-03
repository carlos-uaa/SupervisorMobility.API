using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.JobObservationTypeDtos;

namespace SupervisorMobility.API.Models.JobObservationConfigsDtos
{
    public class JobObservationConfigDto
    {
        public int JobObservationConfigId { get; set; }

        //Navigation properties
        public int JobObservationTypeId { get; set; }
        public JobObservationTypeDto JobObservationTypeDto { get; set; }
            = new JobObservationTypeDto();

        public int ChecklistCategoryId { get; set; }
        public ChecklistCategoryDto ChecklistCategoryDto { get; set; }
            = new ChecklistCategoryDto();
    }
}
