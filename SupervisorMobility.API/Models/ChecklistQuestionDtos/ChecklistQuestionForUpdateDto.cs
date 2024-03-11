using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.ChecklistQuestionDtos
{
    public class ChecklistQuestionForUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Prompt { get; set; } = string.Empty;

        public List<int>? Pillars { get; set; }
        [MaxLength(200)]
        public string NotGood { get; set; } = string.Empty;
        public int CategorySequence { get; set; }
        public bool? IsActive { get; set; }
        //FK
        [Required]
        public int JobCategoryStructureId { get; set; }

        public string PromptEN { get; set; }
        public string NotGoodEN { get; set; }
    }
}
