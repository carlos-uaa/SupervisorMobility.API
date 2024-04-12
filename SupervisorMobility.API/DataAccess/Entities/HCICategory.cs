using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class HCICategory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int HCICategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? Date { get; set; }
        public bool? IsActive { get; set; }
    }
}