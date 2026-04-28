namespace SupervisorMobility.API.Models.HRIRevisionItemsDtos
{
    public class UpdateHRIRevisionItemDto
    {
        public int ItemNumber { get; set; }
        public string RevisionPoint { get; set; }
        public int? RevisionMethodId { get; set; }
        public int? VeredictId { get; set; }
        public int? FrequencyId { get; set; }
    }
}
