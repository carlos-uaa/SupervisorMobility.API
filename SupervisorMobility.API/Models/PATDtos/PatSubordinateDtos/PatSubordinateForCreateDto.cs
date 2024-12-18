namespace SupervisorMobility.API.Models.PATDtos.PatSubordinateDtos
{
    public class PatSubordinateForCreateDto
    {
        public int PatId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
