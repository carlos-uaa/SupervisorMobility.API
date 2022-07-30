namespace SupervisorMobility.API.Models.ChecklistQuestionDtos
{
    public class ChecklistQuestionWithoutNavigationPropertiesDto
    {
        public int QuestionID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
        public int CategorySequence { get; set; }
        public int AnswerSetID { get; set; }
        public bool IsActive { get; set; }
        //FK
        public int ChecklistCategoryId { get; set; }
        public int QuestionTypeId { get; set; }
    }
}
