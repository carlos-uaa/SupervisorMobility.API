namespace SupervisorMobility.API.Models.HRIDtos
{
    public class HRIHistoryItemDto
    {
        public int HRIid { get; set; }
        public int? ResponsibleUserId { get; set; }
        public string? Action { get; set; }
        public DateTime? ActionDate { get; set; }
    }
}
