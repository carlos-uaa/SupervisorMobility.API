using SupervisorMobility.API.Models.HCITransactionDtos;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Models.HCIDtos
{
    public class CreateHCIDto
    {
        public string? HCIName { get; set; }
        public string? HCISectionName { get; set; }
        public int? HCINo { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<CreateHCITransactionDto>? Transactions { get; set; }
          = new List<CreateHCITransactionDto>();

        public ICollection<CreateCommentaryDto>? Comments { get; set; }
          = new List<CreateCommentaryDto>();
        public bool? IsActive { get; set; }

    }
}
