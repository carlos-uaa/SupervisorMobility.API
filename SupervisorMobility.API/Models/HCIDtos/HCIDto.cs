using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.ILU;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.HCICategoryDtos;
using SupervisorMobility.API.Models.HCITransactionDtos;
using SupervisorMobility.API.Models.ILURegisterDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.HCIDtos
{
    public class HCIDto
    {
        public int HCIId { get; set; }
        public string? HCIName { get; set; }
        public string? HCISectionName { get; set; }
        public int? HCINo { get; set; }
        public int? SOSHubId { get; set; }
        public int? UserId { get; set; }
        public UsersWithNavigationAndPeopleDetails? User { get; set; }

        public ICollection<HCITransactionDto>? Transactions { get; set; }
          = new List<HCITransactionDto>();
        public ICollection<HCICategoryDto>? Categories { get; set; }
          = new List<HCICategoryDto>();
        public ICollection<ILURegisterDto>? ILUs { get; set; } = new List<ILURegisterDto>();
        public ICollection<UserCareerPath>? CareerPaths { get; set; } = new List<UserCareerPath>();
        public ICollection<CommentaryDto>? Commentaries { get; set; }
          = new List<CommentaryDto>();
        public bool? IsActive { get; set; }

    }
}
