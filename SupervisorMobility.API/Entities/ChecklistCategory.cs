using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class ChecklistCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChecklistCategoryId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        [Required]
        public int Sequence { get; set; }
        public bool? IsActive { get; set; }

        //Navigation property
        public ICollection<ChecklistQuestion> ChecklistQuestions { get; set; }
            = new List<ChecklistQuestion>();

        public ChecklistCategory(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
