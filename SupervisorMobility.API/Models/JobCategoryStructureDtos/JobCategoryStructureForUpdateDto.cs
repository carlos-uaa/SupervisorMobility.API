using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.ChecklistCategoryDtos
{
    public class JobCategoryStructureForUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        public StructureType Type { get; set; }

        public bool IsActive { get; set; }
    }
}
