using SupervisorMobility.API.Models.QuestionTypeDtos;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.ChecklistQuestionDtos
{
    public class ChecklistQuestionForCreationDto
    {
        [Required]
        [MaxLength(200)]
        public string Prompt { get; set; } = string.Empty;

        public List<int>? Pillars { get; set; }
        [MaxLength(200)]
        public string NotGood { get; set; } = string.Empty;
        public int CategorySequence { get; set; }
        public int JobCategoryStructureId { get; set; }

        public int TypeId { get; set; }

        public List<string>? Options { get; set; }
        public List<string>? Actions { get; set; }

        public bool? IsActive { get; set; }

        public string PromptEN { get; set; }
        public string NotGoodEN { get; set; }

    }
}
