using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;

namespace SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsLogbookDtos
{
    public class SOSSynopticRequirementsLogbookForCreateDto
    {
        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int SOSSynopticRequirementsId { get; set; }
       

        public int? ApproverId { get; set; }
       
        public int? ReviewerId { get; set; }
   }
}
