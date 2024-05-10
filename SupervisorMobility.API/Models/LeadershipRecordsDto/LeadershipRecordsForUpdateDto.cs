using SupervisorMobility.API.Models.DistributionDtos;

namespace SupervisorMobility.API.Models.ILURegisterDtos
{
    public class LeadershipRecordsForUpdateDto
    {
        public int LeadershipRecordsid { get; set; }

        public DateTime? AcquisitionDate { get; set; } = DateTime.Now;

        //public int? OperationId { get; set; }
        public int? DistributionId { get; set; }

        public int? OperatorId { get; set; }

        public int? ILULevelId { get; set; }
        public bool isActive { get; set; }
    }
}
