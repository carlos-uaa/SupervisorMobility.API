
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.Models.ChecklistAnswerDtos
{
    public class ChecklistAnswerDto
    {
        public int AnswerId { get; set; }
        public int? JobObservationId { get; set; }
        public int? QuestionID { get; set; }
        public string? Prompt { get; set; } = string.Empty;
        public string? Answer { get; set; } = string.Empty;
    }
}
