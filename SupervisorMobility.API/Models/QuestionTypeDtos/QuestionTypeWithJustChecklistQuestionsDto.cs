using SupervisorMobility.API.Models.ChecklistQuestionDtos;

namespace SupervisorMobility.API.Models.QuestionTypeDtos
{
    public class QuestionTypeWithJustChecklistQuestionsDto
    {
        public int QuestionTypeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        //Navigation property
        public ICollection<ChecklistQuestionWithoutNavigationPropertiesDto> ChecklistQuestions { get; set; }
            = new List<ChecklistQuestionWithoutNavigationPropertiesDto>();
    }
}
