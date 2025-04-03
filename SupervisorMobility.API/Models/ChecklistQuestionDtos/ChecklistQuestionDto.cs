using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.PillarDtos;
using SupervisorMobility.API.Models.QuestionTypeDtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public int TypeId { get; set; }
        public QuestionTypeDto Type { get; set; }

        public List<string>? Options { get; set; }
        public List<string>? Actions { get; set; }

        public bool? IsActive { get; set; }
        //Navigation properties
        public int JobCategoryStructureId { get; set; }
        public JobCategoryStructureDto JobCategoryStructureDto { get; set; }
            = new JobCategoryStructureDto();

    }
}
