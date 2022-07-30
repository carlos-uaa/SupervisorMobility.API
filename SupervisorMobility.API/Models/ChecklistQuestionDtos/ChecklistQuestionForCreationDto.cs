using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.ChecklistQuestionDtos
{
    public class ChecklistQuestionForCreationDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Prompt { get; set; } = string.Empty;
        [Required]
        public int CategorySequence { get; set; }
        public int? AnswerSetID { get; set; }
        public bool? IsActive { get; set; }
        //FK
        [Required]
        public int QuestionTypeId { get; set; }
    }
}
