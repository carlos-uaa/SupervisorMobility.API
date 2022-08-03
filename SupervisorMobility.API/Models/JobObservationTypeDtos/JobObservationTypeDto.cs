using SupervisorMobility.API.Models.JobObservationConfigsDtos;

namespace SupervisorMobility.API.Models.JobObservationTypeDtos
{
    public class JobObservationTypeDto
    {
        public int JobObservationTypeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public ICollection<JobObservationConfigDto> JobObservations { get; set; }
            = new List<JobObservationConfigDto>();
    }
}
