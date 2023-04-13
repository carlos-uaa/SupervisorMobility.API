namespace SupervisorMobility.API.Models.AttendanceDtos
{
    public class AttendanceForUpdateDto
    {
        public string? Name { get; set; }
        public int? Payroll { get; set; }
        public int? AreaId { get; set; }
        public int? GroupId { get; set; }
        public bool Compas { get; set; }
        public bool Station { get; set; }
    }
}
