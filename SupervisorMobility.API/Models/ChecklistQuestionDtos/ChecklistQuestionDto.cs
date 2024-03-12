using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.PillarDtos;
using SupervisorMobility.API.Models.QuestionTypeDtos;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.ChecklistQuestionDtos
{
    public class ChecklistQuestionDto
    {
        public int QuestionID { get; set; }
        public string Prompt { get; set; }
        public List<PillarDto>? Pillars { get; set; }
        public string NotGood { get; set; }
        public int CategorySequence { get; set; }
        public string PromptEN { get; set; }
        public string NotGoodEN { get; set; }

        public bool? IsActive { get; set; }
        //Navigation properties
        public int JobCategoryStructureId { get; set; }
        public JobCategoryStructureDto JobCategoryStructureDto { get; set; }
            = new JobCategoryStructureDto();

    }
}
