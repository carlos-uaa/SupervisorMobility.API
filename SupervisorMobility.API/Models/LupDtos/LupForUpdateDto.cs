using SupervisorMobility.API.Models.ChecklistQuestionDtos;

namespace SupervisorMobility.API.Models.LupDtos
{
    public class LupForUpdateDto
    {
        public int JobObservationId { get; set; }
        public string? Oportunity { get; set; }
        public bool? IsActive { get; set; }
        public string? Observer { get; set; }
        public int Pillar { get; set; }
        public string? Q3 { get; set; }
        public string? Q4 { get; set; }
        public string? Justification { get; set; }
        public int? Status { get; set; }
        public LUPStatus? StatusOKNG { get; set; }


        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DepartmentId { get; set; }
        public int? StdChange { get; set; }
        public int? StdUpdate { get; set; }
        public int? ChecklistQuestionId { get; set; }
    }
}
