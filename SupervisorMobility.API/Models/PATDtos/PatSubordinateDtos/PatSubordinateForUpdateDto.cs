using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Models.PATDtos.PatSubordinateDtos
{
    public class PatSubordinateForUpdateDto
    {
        public int PatSubordinateId { get; set; }
        public int PatId { get; set; }
        public int UserId { get; set; }
        public List<PatSubordinateDates> PatSubordinateDates { get; set; } = new List<PatSubordinateDates>();
    }
}
