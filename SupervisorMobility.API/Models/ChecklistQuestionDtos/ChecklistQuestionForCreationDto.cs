using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.ChecklistQuestionDtos
{
    public class ChecklistQuestionForCreationDto
    {
        [Required]
        [MaxLength(200)]
        public string Prompt { get; set; } = string.Empty;

        [Required]
        public int PillarId { get; set; }
        [Required]
        public int Sequence { get; set; }
        [MaxLength(200)]
        public string NotGood { get; set; } = string.Empty;
        public int CategorySequence { get; set; }

        public bool? IsActive { get; set; }

    }
}
