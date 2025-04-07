using SupervisorMobility.API.Models.ChecklistQuestionDtos;

namespace SupervisorMobility.API.Models.ChecklistCategoryDtos
{
    public class JobCategoryStructureWithJustchecklistQuestionsDto
    {
        public int JobCategoryStructureId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public StructureType Type { get; set; }

        public bool IsActive { get; set; }

        //Navigation property
        public ICollection<ChecklistQuestionsWithType> ChecklistQuestions { get; set; }
            = new List<ChecklistQuestionsWithType>();
    }
}
