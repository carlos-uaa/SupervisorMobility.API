using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.HCITransactionDtos;
using SupervisorMobility.API.Models.CommentaryDtos;

namespace SupervisorMobility.API.Models.HCIDtos
{
    public class UpdateHCIDto
    {
        public int HCIId { get; set; }
        public string? HCIName { get; set; }
        public string? HCISectionName { get; set; }
        public int? HCINo { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public ICollection<UpdateHCITransactionDto>? Transactions { get; set; }
          = new List<UpdateHCITransactionDto>();
        public ICollection<UpdateCommentaryDto>? Comments { get; set; }
          = new List<UpdateCommentaryDto>();
        public bool? IsActive { get; set; }

    }
}
