using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class HCI
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HCIId { get; set; }

        public string? HCIName { get; set; }
        public string? HCISectionName { get; set; }
        public int? HCINo { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<HCITransaction>? Transactions { get; set; }
          = new List<HCITransaction>();

        public ICollection<Commentary>? Comments { get; set; }
          = new List<Commentary>();
        public bool? IsActive { get; set; }
    }
}
