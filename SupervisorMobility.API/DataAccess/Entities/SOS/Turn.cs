using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class Turn
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TurnId { get; set; }

        public string? TurnType { get; set; }

        public int? OperatorId { get; set; }
        public User? Operator { get; set; }

        public int? SupervisorId { get; set; }
        public User? Supervisor { get; set; }

    }
}