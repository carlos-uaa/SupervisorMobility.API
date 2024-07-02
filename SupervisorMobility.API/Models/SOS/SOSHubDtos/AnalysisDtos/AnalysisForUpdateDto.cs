namespace SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisDtos
{
    public class AnalysisForUpdateDto
    {
        public int AnalysisId { get; set; }
        public string? Text { get; set; }
        public string? CriticalPoint { get; set; } = string.Empty;
        public string?   Reason { get; set; }
        public bool? IsActive { get; set; }
    }
}
