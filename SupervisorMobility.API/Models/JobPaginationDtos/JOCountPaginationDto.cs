namespace SupervisorMobility.API.Models.JobPaginationDtos
{
    public class JOCountPaginationDto
    {
        public List<JOCount> DistributionCount { get; set; }
        public List<JOCount> OperationCount { get; set; }
        public List<JOCount> OperatorCount { get; set; }
        public List<JOCount> StatusCount { get; set; }
    }


    public class JOCount
    {
        public int id { get; set; }
        public int count { get; set; }
    }
}
