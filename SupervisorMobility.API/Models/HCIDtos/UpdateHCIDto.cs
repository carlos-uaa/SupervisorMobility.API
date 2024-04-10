using SupervisorMobility.API.Models.HCITransactionDtos;

namespace SupervisorMobility.API.Models.HCIDtos
{
    public class UpdateHCIDto
    {
        public int HCIId { get; set; }

        public string HCIName { get; set; }

        public string HCISectionName { get; set; }

        public int HCINo { get; set; }

        public int? UserId { get; set; }

        public ICollection<UpdateHCITransactionDto> Transactions { get; set; }
           = new List<UpdateHCITransactionDto>();

        public ICollection<string> Comentarys { get; set; }
           = new List<string>();
        public bool? IsActive { get; set; }

    }
}
