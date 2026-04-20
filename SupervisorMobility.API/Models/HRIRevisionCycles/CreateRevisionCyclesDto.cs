namespace SupervisorMobility.API.Models.HRIRevisionCycles
{
    public class CreateRevisionCyclesDto
    {       
        public int Cycle { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
