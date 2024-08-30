using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using SupervisorMobility.API.Models.SOS.SOSCombinationDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.FileUploadDto;

namespace SupervisorMobility.API.Models.SOS.SOSCombinationLogbookDtos
{
    public class SOSCombinationLogbookForUpdateDto
    {
        public int SOSCombinationLogbookId { get; set; }
        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int SOSCombinationId { get; set; }

        public int? ApproverId { get; set; }

        public int? ReviewerId { get; set; }

    }
}
