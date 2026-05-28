using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionsItem_Entities;

namespace SupervisorMobility.API.Models.HRIRevisionItemsDtos
{
    public class UpdateRevisionItemDto
    {
        public int ItemId { get; set; }
        public bool? Deleted { get; set; }
        public int HriId { get; set; }
        public int ItemNumber { get; set; }
        public string RevisionPoint { get; set; }
        public int? RevisionMethodId { get; set; }
        public int? VeredictId { get; set; }
        public int? FrequencyId { get; set; }
        public bool? IsActive { get; set; }
    }
}
