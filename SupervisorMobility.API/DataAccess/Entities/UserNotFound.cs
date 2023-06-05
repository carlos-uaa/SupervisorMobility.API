using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;
using System.Text.Json.Serialization;
using SupervisorMobility.API.DataAccess.Entities.ILU;

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
