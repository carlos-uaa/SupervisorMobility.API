using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class HeadCountProcess
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HeadCountProcessId { get; set; }
      
        public string Process { get; set; }
    }
}
