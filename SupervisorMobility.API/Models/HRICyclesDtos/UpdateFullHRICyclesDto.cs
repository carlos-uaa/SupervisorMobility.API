using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.Models.HRICyclesDtos
{
    public class UpdateFullHRICyclesDto
    {
        public int CycleId { get; set; }
        public int Cycle { get; set; }
        public bool? Deleted { get; set; }
        public int HriId { get; set; }
        public int? SupervisorUserId { get; set; }
        public int? OperadorUserIdId { get; set; }
        public string? UserType { get; set; }
        public bool? IsActive { get; set; }
    }
}
