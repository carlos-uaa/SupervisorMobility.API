using SupervisorMobility.API.Models.ChecklistQuestionDtos;
using SupervisorMobility.API.Models.JobObservationConfigsDtos;

namespace SupervisorMobility.API.Models.ChecklistCategoryDtos
{
    public class ChecklistCategoryDto
    {
        public int ChecklistCategoryId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public bool IsActive { get; set; }

        //Navigation property
        public ICollection<ChecklistQuestionDto> ChecklistQuestions { get; set; }
            = new List<ChecklistQuestionDto>();

        public ICollection<JobObservationConfigDto> JobObservationConfigs { get; set; }
            = new List<JobObservationConfigDto>();
    }
}
