namespace SupervisorMobility.API.Models.SOS.SOSDistributionAdditionalTimeDtos
{
    public class SOSDistributionAdditionalTimeForUpdateDto
    {
        public int SOSDistributionAdditionalTimeId { get; set; }
        public string? TakeQuantity { get; set; } = "§§§§";
        public string? TakeTime { get; set; } = "§§§§§";
        public string? LeaveQuantity { get; set; } = "§§§§";
        public string? LeaveTime { get; set; } = "§§§§§";
        public string? StepsQuantity { get; set; } = "§§§§";
        public string? StepsTime { get; set; } = "§§§§§";
        public bool? IsActive { get; set; }

    }
}
