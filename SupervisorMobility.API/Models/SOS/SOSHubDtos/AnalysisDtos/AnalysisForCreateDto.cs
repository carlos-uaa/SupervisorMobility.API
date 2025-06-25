namespace SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisDtos
{
    public class AnalysisForCreateDto
    {
        public string? Text { get; set; }
        public string Uid { get; set; }
        public List<string>? CriticalPoints { get; set; } = new List<string>();
        public List<string>? Reasons { get; set; } = new List<string>();
        public bool? IsActive { get; set; }
    }
}
