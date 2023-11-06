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
        [MaxLength(200)]
        public string Prompt { get; set; }

        [Required]
        public int PillarId { get; set; }

        [Required]
        public int Sequence { get; set; }
        [MaxLength(200)]
        public string NotGood { get; set; }

        public int CategorySequence { get; set; }

        public bool? IsActive { get; set; }


        //Navigation properties
        public int ChecklistCategoryId { get; set; }
        public ChecklistCategory? ChecklistCategory { get; set; }

    }
}
