namespace SupervisorMobility.API.Models.SOS.SOSTimeDtos
{
    public class SOSTimeForUpdateDto
    {
        public int SOSTimeId { get; set; }
        public int SectionId { get; set; }
        public int? AnalysisId { get; set; }
        public string? Time { get; set; } = "";

        public bool? IsActive { get; set; }
    }
}
