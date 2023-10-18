using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class UserNotFound
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserNotFoundId { get; set; }
        public string? ObjectId { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
