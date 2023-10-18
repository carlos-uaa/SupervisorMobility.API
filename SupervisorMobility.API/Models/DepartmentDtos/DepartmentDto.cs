namespace SupervisorMobility.API.Models.DepartmentDtos
{
    public class DepartmentDto
    {
        public int DepartmentId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
