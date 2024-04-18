using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class KaizenTransaction
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? KaizenTransactionId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Type {  get; set; }
        public bool? IsActive { get; set; }

    }
}
