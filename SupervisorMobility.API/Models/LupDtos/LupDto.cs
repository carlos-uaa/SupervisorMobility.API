using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.JobObservationDtos;
using SupervisorMobility.API.Models.PlantDtos;

namespace SupervisorMobility.API.Models.LupDtos
{
    public class LupDto
    {
        public int LupId { get; set; }

        public JobObservationDto JobObservation { get; set; } = new JobObservationDto();
        public int? JobObservationId { get; set; }


        public bool? IsActive { get; set; }

        public string? Observer { get; set; }
        public int Pillar { get; set; }
        public string? Q3 { get; set; }
        public string? Q4 { get; set; }
        public string? Evidence { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
