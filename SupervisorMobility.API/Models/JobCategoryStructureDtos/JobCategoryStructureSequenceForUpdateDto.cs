using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.ChecklistCategoryDtos
{
    public class JobCategoryStructureSequenceForUpdateDto
    {
        [Required]
        public int Sequence { get; set; }
    }
}
