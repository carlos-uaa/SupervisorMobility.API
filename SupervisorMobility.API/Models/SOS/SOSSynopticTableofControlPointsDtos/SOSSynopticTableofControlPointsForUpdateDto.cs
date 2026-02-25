namespace SupervisorMobility.API.Models.SOS.SOSSynopticTableofControlPointsDtos
{
    public class SOSSynopticTableofControlPointsForUpdateDto
    {
        public int SOSSynopticTableofControlPointsId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? ProcessName { get; set; }

        public int? CreatorId { get; set; }
        public int? ReviewerId { get; set; }
        public int? ApproverId { get; set; }

        public bool? IsActive { get; set; }
        public int? SOSHubId { get; set; }
    }
}
