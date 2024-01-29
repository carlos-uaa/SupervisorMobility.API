using SupervisorMobility.API.Models.ChecklistCategoryDtos;

namespace SupervisorMobility.API.Models.ChecklistQuestionDtos
{
    public class ChecklistQuestionsWithoutQuestionType
    {
        public int QuestionID { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public int PillarId { get; set; }
        public string NotGood { get; set; } = string.Empty;
        public int CategorySequence { get; set; }

        public bool? IsActive { get; set; }

        //Navigation properties
        public int ChecklistCategoryId { get; set; }
        public JobCategoryStructureDto JobCategoryStructureDto { get; set; }
            = new JobCategoryStructureDto();
    }
}
