using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.Models.LupDtos
{
    public class LupWithoutNavigationPropertiesDto
    {
        public int LupId { get; set; }
        public int JobObservationId { get; set; }
        public string? Oportunity { get; set; }
        public bool? IsActive { get; set; }

        public string? Observer { get; set; }
        public int Pillar { get; set; }
        public string? Q3 { get; set; }
        public string? Q4 { get; set; }
        public string? Justification { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }

    }
}
