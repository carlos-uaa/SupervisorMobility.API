namespace SupervisorMobility.API.Models.PATDtos.PatDistributionCommentDtos
{
    public class PatDistributionCommentDto
    {
        public int PatDistributionCommentId { get; set; }
        public int PATId { get; set; }
        public int DistributionId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
