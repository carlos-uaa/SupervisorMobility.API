namespace SupervisorMobility.API.Models.HRIHourmeterRevisionDto
{
    public class CreateHourMeterRevisionDto
    {
        public int? HriId { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
