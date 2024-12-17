using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.PATDtos.PatUserRoleDtos
{
    public class PatUserRoleDto
    {
        public int PatUserRole { get; set; }
        public int PATId { get; set; }
        public int UserId { get; set; }
        public OperatorRole? Role { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; }
    }
}
