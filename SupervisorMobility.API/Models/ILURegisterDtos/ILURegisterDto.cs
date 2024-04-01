using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.ILU;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.ILURegisterDtos
{
    public class ILURegisterDto
    {
        public int ILURegisterid { get; set; }

        public DateTime? AcquisitionDate { get; set; } = DateTime.Now;

        //public int? OperationId { get; set; }
        //public OperationWithoutNavigationPropertiesDto? Operation { get; set; }
        public int? DistributionId { get; set; }
        public DistributionWithoutNavigationPropertiesDto? Distribution { get; set; }


        public int? OperatorId { get; set; }
        public UsersWithoutNavigationWithoutPeopleDetails? Operator { get; set; }


        public int? ILULevelId { get; set; }
        public ILULevelDto? ILULevel { get; set; }
        public bool isActive { get; set; }
    }
}
