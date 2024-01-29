using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum StructureType
{
    Titular,
    Checklist,
    Timer,
    LUP,
    Signature
}

namespace SupervisorMobility.API.Entities
{
    [Index(nameof(Code), IsUnique = true, Name = "ix_cc_cod")]
    public class JobCategoryStructure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobCategoryStructureId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        [Required]
        public int Sequence { get; set; }

        public StructureType Type { get; set; }

        public bool? IsActive { get; set; }

        //Navigation property
        public ICollection<ChecklistQuestion> ChecklistQuestions { get; set; }
            = new List<ChecklistQuestion>();

        public JobCategoryStructure(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
