using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class HCITransaction
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int HCITransactionId { get; set; }

        public string? Description { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? DateStart { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? DateEnd { get; set; }
        public int Type { get; set; }
        public bool? IsActive { get; set; }
    }
}