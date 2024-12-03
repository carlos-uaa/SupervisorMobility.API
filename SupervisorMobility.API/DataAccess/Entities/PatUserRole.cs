using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public enum OperatorRole
    {
        SV,
        Lider,
        CA, // C/A
        NI
    }

    public class PatUserRole
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatUserRoleId { get; set; }
        public int PATId { get; set; }
        public int UserId { get; set; }
        public OperatorRole? Role { get; set; }
        public bool isActive { get; set; }

    }
}
