using SupervisorMobility.API.Models.ILURegisterDtos;

namespace SupervisorMobility.API.Models.Users
{
    public class UserNotFoundForCreation
    {
        public string? ObjectId { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
