using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.GroupDtos;

namespace SupervisorMobility.API.Models.AttendanceDtos
{
    public class AttendanceWithoutDetailsDto
    {
        public int AttendanceId { get; set; }
        public string? Name { get; set; }
        public int? Payroll { get; set; }
        public int? AreaId { get; set; }
        public int? GroupId { get; set; }
        public bool Compas { get; set; }
        public bool Station { get; set; }
    }
}
