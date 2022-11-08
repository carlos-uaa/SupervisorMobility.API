using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class SupportDocumentType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupportDocumentTypeId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        public SupportDocumentType(string description, string code)
        {
            Description = description;
            Code = code;
        }


    }
}
