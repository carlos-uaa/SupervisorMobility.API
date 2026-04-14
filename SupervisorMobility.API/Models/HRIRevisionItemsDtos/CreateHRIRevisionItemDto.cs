using SupervisorMobility.API.DataAccess.Entities.HRI;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionsItem_Entities;

namespace SupervisorMobility.API.Models.HRIRevisionItemsDtos
{
    public class CreateHRIRevisionItemDto
    {

        public int HriId { get; set; }
        public int ItemNumber { get; set; }
        public string RevisionPoint { get; set; }
        public int? RevisionMethodId { get; set; } 
        public int? VeredictId { get; set; }
        public int? FrequencyId { get; set; }
       
    }
}
