using SupervisorMobility.API.Models.JobObservationDtos;

namespace SupervisorMobility.API.Models.JobPaginationDtos
{
    public class JOPaginationDto
    {
        public int Total { get; set; }
        public IEnumerable<JobObservationDto> JobObservations { get; set; }
        public JOCountPaginationDto CountPagination { get; set; }
    }
}
