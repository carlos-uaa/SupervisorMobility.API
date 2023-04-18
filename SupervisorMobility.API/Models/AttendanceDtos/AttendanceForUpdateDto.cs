namespace SupervisorMobility.API.Models.AttendanceDtos
{
    public class AttendanceForUpdateDto
    {
        public string? Name { get; set; }
        public int? SuperiorId { get; set; }
        public int? UserId { get; set; }
        public int? CurrentdistributionId { get; set; }
        public bool Compas { get; set; }
        public bool Station { get; set; }
    }
}
