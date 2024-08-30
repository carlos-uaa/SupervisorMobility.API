using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using SupervisorMobility.API.Models.SOS.SOSFlowDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.FileUploadDto;

namespace SupervisorMobility.API.Models.SOS.SOSFlowLogbookDtos
{
    public class SOSFlowLogbookForUpdateDto
    {
        public int SOSFlowLogbookId { get; set; }
        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int SOSFlowId { get; set; }

        public int? ApproverId { get; set; }

        public int? ReviewerId { get; set; }

    }
}
