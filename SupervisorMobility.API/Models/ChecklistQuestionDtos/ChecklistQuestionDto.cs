using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.QuestionTypeDtos;

namespace SupervisorMobility.API.Models.ChecklistQuestionDtos
{
    public class ChecklistQuestionDto
    {
        public int QuestionID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
        public int CategorySequence { get; set; }
        public int AnswerSetID { get; set; }
        public bool IsActive { get; set; }

        //Navigation properties
        public int ChecklistCategoryId { get; set; }
        public ChecklistCategoryDto ChecklistCategoriesDto { get; set; }
            = new ChecklistCategoryDto();

        public int QuestionTypeId { get; set; }
        public QuestionTypeDto QuestionTypesDto { get; set; }
            = new QuestionTypeDto();
    }
}
