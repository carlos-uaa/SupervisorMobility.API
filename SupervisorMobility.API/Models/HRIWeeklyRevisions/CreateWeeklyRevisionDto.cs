using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.Models.HRIWeeklyRevisions
{
    public class CreateWeeklyRevisionDto
    {
        public int HriId { get; set; }      
        public int? UserId { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
