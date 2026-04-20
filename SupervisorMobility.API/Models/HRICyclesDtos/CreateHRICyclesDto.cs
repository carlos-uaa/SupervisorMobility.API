using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;


namespace SupervisorMobility.API.Models.HRICyclesDtos
{
    public class CreateHRICyclesDto
    {
        public int HriId { get; set; }
        public int Cycle { get; set; }
        public int? UserId { get; set; }
        public string? UserType { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
