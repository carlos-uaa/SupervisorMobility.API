using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Models.PATDtos.PatSubordinateDtos
{
    public class PatSubordinateForCreateDto
    {
        public int PatId { get; set; }
        public int UserId { get; set; }
        public List<PatSubordinateDates> PatSubordinateDates { get; set; } = new List<PatSubordinateDates>();
    }
}
