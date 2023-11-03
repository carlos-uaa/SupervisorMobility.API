using SupervisorMobility.API.Models.ChecklistCategoryDtos;
using SupervisorMobility.API.Models.QuestionTypeDtos;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.ChecklistQuestionDtos
{
    public class ChecklistQuestionDto
    {
        public int QuestionID { get; set; }
        public string Prompt { get; set; }
        public int PillarId { get; set; }
        public int Sequence { get; set; }
        public string NotGood { get; set; }
        public int CategorySequence { get; set; }

        public bool? IsActive { get; set; }
        //Navigation properties
        public int ChecklistCategoryId { get; set; }
        public ChecklistCategoryDto ChecklistCategoriesDto { get; set; }
            = new ChecklistCategoryDto();

    }
}
