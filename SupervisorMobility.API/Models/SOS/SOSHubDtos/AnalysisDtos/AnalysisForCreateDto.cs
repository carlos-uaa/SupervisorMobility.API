namespace SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisDtos
{
    public class AnalysisForCreateDto
    {
        public string? Text { get; set; }
        public string? CriticalPoint { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public bool? IsActive { get; set; }
    }
}
