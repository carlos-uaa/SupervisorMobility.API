namespace SupervisorMobility.API.Models.JobObservationTypeDtos
{
    public class JobObservationTypeWithoutConfigsDto
    {
        public int JobObservationTypeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
