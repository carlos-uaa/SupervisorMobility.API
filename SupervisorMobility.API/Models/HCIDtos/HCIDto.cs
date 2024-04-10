using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Models.HCITransactionDtos;

namespace SupervisorMobility.API.Models.HCIDtos
{
    public class HCIDto
    {
        public int HCIId { get; set; }

        public string? HCIName { get; set; }
        public string? HCISectionName { get; set; }
        public int? HCINo { get; set; }

        public User? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<HCITransactionDto>? Transactions { get; set; }
          = new List<HCITransactionDto>();

        public ICollection<string>? Comentarys { get; set; }
          = new List<string>();
        public bool? IsActive { get; set; }

    }
}
