namespace SupervisorMobility.API.Models.ChecklistAnswerDtos
{
    public class ChecklistAnswerForCreationDto
    {
        public int JobObservationId { get; set; }
        public int QuestionID { get; set; }
        public string? Prompt { get; set; } = string.Empty;
        public string? Answer { get; set; } = string.Empty;
    }
}
