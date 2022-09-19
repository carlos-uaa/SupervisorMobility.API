using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class QuestionType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionTypeId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        //Navigation property
        public ICollection<ChecklistQuestion> ChecklistQuestions { get; set; }
            = new List<ChecklistQuestion>();

        public QuestionType(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
