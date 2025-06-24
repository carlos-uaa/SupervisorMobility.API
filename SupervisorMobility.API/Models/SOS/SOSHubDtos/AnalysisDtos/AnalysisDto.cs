namespace SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisDtos
{
    public class AnalysisDto
    {
        public int AnalysisId { get; set; }
        public string Uid { get; set; }
        public string? Text { get; set; }
        public List<string>? CriticalPoints { get; set; } = new List<string>();
        public List<string>? Reasons { get; set; } = new List<string>();
        public bool? IsActive { get; set; }
    }
}
