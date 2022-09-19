using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class ChecklistQuestion
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        [Required]
        [MaxLength(200)]
        public string Prompt { get; set; }
        [Required]
        public int CategorySequence { get; set; }
        public int? AnswerSetID { get; set; }
        public bool? IsActive { get; set; }

        //Navigation properties
        public int ChecklistCategoryId { get; set; }
        public ChecklistCategory? ChecklistCategory { get; set; }

        public int QuestionTypeId { get; set; }
        public QuestionType? QuestionType { get; set; }

        public ChecklistQuestion(string code, string description, string prompt)
        {
            Code = code;
            Description = description;
            Prompt = prompt;
        }

    }
}
