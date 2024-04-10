using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class HCI
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HCIId { get; set; }

        public string? HCIName {  get; set; }
        public string? HCISectionName {  get; set; }
        public int? HCINo {  get; set; }

        public User? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<HCITransaction>? Transactions { get; set; }
          = new List<HCITransaction>();
        
        public ICollection<string>? Comentarys { get; set; }
          = new List<string>();
        public bool? IsActive { get; set; }

    }
}
