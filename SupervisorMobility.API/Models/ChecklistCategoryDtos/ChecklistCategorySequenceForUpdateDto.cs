using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.ChecklistCategoryDtos
{
    public class ChecklistCategorySequenceForUpdateDto
    {
        [Required]
        public int Sequence { get; set; }
    }
}
