using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.GroupDtos;

namespace SupervisorMobility.API.Models.AttendanceDtos
{
    public class AttendanceWithNavigationDetailsDto
    {
        public int AttendanceId { get; set; }
        public string? Name { get; set; }
        public int? Payroll { get; set; }
        public int? AreaId { get; set; }
        public AreaWithoutNavigationPropertiesDto? Area { get; set; } = new AreaWithoutNavigationPropertiesDto();
        public GroupDto? Group { get; set; } = new GroupDto();
        public int? GroupId { get; set; }
        public bool Compas { get; set; }
        public bool Station { get; set; }
    }
}
