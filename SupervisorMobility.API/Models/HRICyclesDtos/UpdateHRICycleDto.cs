

using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.Models.HRICyclesDtos
{
    public class UpdateHRICycleDto
    {

        public int CycleId { get; set; }
        public int Cycle { get; set; }
        public int HriId { get; set; }
        public int? SupervisorUserId { get; set; }
        public int? OperatorUserId { get; set; }
        public string? UserType { get; set; }
        public bool? IsActive { get; set; }

    }
}
