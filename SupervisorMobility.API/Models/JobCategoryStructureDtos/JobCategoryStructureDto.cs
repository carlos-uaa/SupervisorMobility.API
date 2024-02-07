using SupervisorMobility.API.Models.ChecklistQuestionDtos;

namespace SupervisorMobility.API.Models.ChecklistCategoryDtos
{
    public class JobCategoryStructureDto
    {
        public int JobCategoryStructureId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public StructureType Type { get; set; }
        public bool IsActive { get; set; }

        //Navigation property
        public ICollection<ChecklistQuestionDto> ChecklistQuestions { get; set; }
            = new List<ChecklistQuestionDto>();

    }
}
