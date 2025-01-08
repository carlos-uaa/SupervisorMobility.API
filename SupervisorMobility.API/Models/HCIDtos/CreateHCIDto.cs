using SupervisorMobility.API.Models.HCITransactionDtos;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.HCICategoryDtos;
using SupervisorMobility.API.DataAccess.Entities.ILU;

namespace SupervisorMobility.API.Models.HCIDtos
{
    public class CreateHCIDto
    {

        public int? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<CreateHCITransactionDto>? Transactions { get; set; }
          = new List<CreateHCITransactionDto>();
     
        public ICollection<HCICategoryDto>? Categories { get; set; }
          = new List<HCICategoryDto>();

        public ICollection<ILURegister>? ILUs { get; set; } = new List<ILURegister>();
        public ICollection<UserCareerPath>? CareerPaths { get; set; } = new List<UserCareerPath>();
        public ICollection<CreateCommentaryDto>? Commentaries { get; set; }
          = new List<CreateCommentaryDto>();
        public bool? IsActive { get; set; }

    }
}
