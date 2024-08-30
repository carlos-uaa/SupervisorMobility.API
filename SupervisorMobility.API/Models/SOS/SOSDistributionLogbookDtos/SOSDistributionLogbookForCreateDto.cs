using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos
{
    public class SOSDistributionLogbookForCreateDto
    {
        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int SOSDistributionId { get; set; }

        public int? ApproverId { get; set; }
        public int? ReviewerId { get; set; }
    }
}
