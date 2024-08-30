using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.FileUploadDto;

namespace SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos
{
    public class SOSSequenceLogbookForUpdateDto
    {
        public int SOSSequenceLogbookId { get; set; }
        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int SOSSequenceId { get; set; }

        public int? ApproverId { get; set; }

        public int? ReviewerId { get; set; }

    }
}
