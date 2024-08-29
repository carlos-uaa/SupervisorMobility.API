namespace SupervisorMobility.API.Models.SOS.SOSTimeDtos
{
    public class SOSTimeForCreateDto
    {
        public int SectionId { get; set; }
        public int? AnalysisId { get; set; }
        public string? Time { get; set; } = "";

        public bool? IsActive { get; set; }
    }
}
