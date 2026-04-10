using SupervisorMobility.API.DataAccess.Entities.HRI;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionsItem_Entities
{
    public class RevisionMethod
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<HRIRevisionItems> HRIRevisionItems { get; set; }    
    }

}
