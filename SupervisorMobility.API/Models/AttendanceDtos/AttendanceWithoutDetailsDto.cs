using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.GroupDtos;

namespace SupervisorMobility.API.Models.AttendanceDtos
{
    public class AttendanceWithoutDetailsDto
    {
        public int AttendanceId { get; set; }
        public int? SuperiorId { get; set; }
        public int? UserId { get; set; }
        public int? CurrentdistributionId { get; set; }
        public bool Compas { get; set; }
        public bool Station { get; set; }
    }
}
