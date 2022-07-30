namespace SupervisorMobility.API.Models.ChecklistCategoryDtos
{
    public class ChecklistCategoryWithJustchecklistQuestions
    {
        public int ChecklistCategoryId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public bool IsActive { get; set; }

        //Navigation property
        public ICollection<ChecklistCategoryWithoutChecklistQuestionsDto> ChecklistQuestions { get; set; }
            = new List<ChecklistCategoryWithoutChecklistQuestionsDto>();
    }
}
