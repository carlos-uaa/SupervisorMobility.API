using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Models.PATDtos.PatUserRoleDtos
{
    public class PatUserRoleDto
    {
        public int PatUserRoleId { get; set; }
        public int PATId { get; set; }
        public int UserId { get; set; }
        public OperatorRole? Role { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; }
    }
}
