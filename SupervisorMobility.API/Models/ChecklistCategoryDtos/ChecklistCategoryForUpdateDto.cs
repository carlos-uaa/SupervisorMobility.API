using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.ChecklistCategoryDtos
{
    public class ChecklistCategoryForUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        [Required]
        public int Sequence { get; set; }
        public bool IsActive { get; set; }
    }
}
