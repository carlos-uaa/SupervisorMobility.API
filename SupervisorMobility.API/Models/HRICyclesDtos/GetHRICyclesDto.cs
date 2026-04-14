using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI;

namespace SupervisorMobility.API.Models.HRICyclesDtos
{
    public class GetHRICyclesDto
    {
        public int CycleId { get; set; }
        public int HriId { get; set; }
        public HRI HRI { get; set; }
        public int Cycle { get; set; }
        public int UserId { get; set; }
        public User? Responsible { get; set; }
        public string? UserType { get; set; }
        public List<DailyRevisions>? DailyRevisions { get; set; }
    }
}
