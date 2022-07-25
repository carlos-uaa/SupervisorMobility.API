namespace SupervisorMobility.API.Models
{
    public class QuestionTypeDto
    {
        public int QuestionTypeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        //Navigation property
        public ICollection<ChecklistQuestionDto> ChecklistQuestions { get; set; }
            = new List<ChecklistQuestionDto>();
    }
}
