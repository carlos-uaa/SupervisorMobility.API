namespace SupervisorMobility.API.Models.ChecklistAnswerDtos
{
    public class ChecklistAnswerForUpdateDto
    {
        public int JobObservationId { get; set; }
        public int? LupId { get; set; }
        public int QuestionID { get; set; }
        public string? Prompt { get; set; } = string.Empty;
        public string? Answer { get; set; } = string.Empty;
    }
}
