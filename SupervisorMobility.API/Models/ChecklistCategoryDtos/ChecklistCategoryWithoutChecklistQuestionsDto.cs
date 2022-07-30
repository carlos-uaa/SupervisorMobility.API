namespace SupervisorMobility.API.Models.ChecklistCategoryDtos
{
    public class ChecklistCategoryWithoutChecklistQuestionsDto
    {
        public int ChecklistCategoryId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public bool IsActive { get; set; }
    }
}
