namespace SupervisorMobility.API.Models.ChecklistCategoryDtos
{
    public class JobCategoryStructureWithoutChecklistQuestionsDto
    {
        public int JobCategoryStructureId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public StructureType Type { get; set; }

        public int Sequence { get; set; }
        public bool IsActive { get; set; }
    }
}
