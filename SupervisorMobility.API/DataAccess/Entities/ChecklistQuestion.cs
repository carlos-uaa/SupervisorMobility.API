using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class ChecklistQuestion
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionID { get; set; }
        [Required]
       
        public string Prompt { get; set; }
        public string? PromptEN { get; set; } = string.Empty;

        [Required]
        public int PillarId { get; set; }

     
        public string NotGood { get; set; }
        public string? NotGoodEN { get; set; } = string.Empty;

        public int CategorySequence { get; set; }

        public bool? IsActive { get; set; }


        //Navigation properties
        public int JobCategoryStructureId { get; set; }
        public JobCategoryStructure? JobCategoryStructure { get; set; }

    }
}
