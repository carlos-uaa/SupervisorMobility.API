using SupervisorMobility.API.Models.ILU;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.ILURegisterDtos
{
    public class ILURegisterForUpdateDto
    {
        public DateTime? AcquisitionDate { get; set; } = DateTime.Now;

        public int? OperationId { get; set; }

        public int? OperatorId { get; set; }

        public int? ILULevelId { get; set; }
    }
}
