using SupervisorMobility.API.Models.ChecklistQuestionDtos;

namespace SupervisorMobility.API.Models.ChecklistCategoryDtos
{
    public class ChecklistCategoryWithJustchecklistQuestionsDto
    {
        public int ChecklistCategoryId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public bool IsActive { get; set; }

        //Navigation property
        public ICollection<ChecklistQuestionWithoutNavigationPropertiesDto> ChecklistQuestions { get; set; }
            = new List<ChecklistQuestionWithoutNavigationPropertiesDto>();
    }
}
