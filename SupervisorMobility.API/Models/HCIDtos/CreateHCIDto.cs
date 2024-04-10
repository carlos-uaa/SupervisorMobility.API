using SupervisorMobility.API.Models.HCITransactionDtos;

namespace SupervisorMobility.API.Models.HCIDtos
{
    public class CreateHCIDto
    {
        public string HCIName { get; set; }

        public string HCISectionName { get; set; }

        public int HCINo { get; set; }

        public int UserId { get; set; }

        public ICollection<CreateHCITransactionDto> Transactions { get; set; }
           = new List<CreateHCITransactionDto>();

        public ICollection<string> Comentarys { get; set; }
           = new List<string>();
        public bool? IsActive { get; set; }

    }
}
