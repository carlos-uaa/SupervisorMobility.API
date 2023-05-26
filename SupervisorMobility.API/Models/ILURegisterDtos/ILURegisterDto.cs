using SupervisorMobility.API.DataAccess.Entities.LUP;
using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.ILU;

namespace SupervisorMobility.API.Models.ILURegisterDtos
{
    public class ILURegisterDto
    {
        public int ILURegisterid { get; set; }

        public DateTime? AcquisitionDate { get; set; } = DateTime.Now;

        public int? OperationId { get; set; }
        public OperationWithoutNavigationPropertiesDto? Operation { get; set; }


        public int? OperatorId { get; set; }
        public UsersWithoutNavigationDetails? Operator { get; set; }


        public int? ILULevelId { get; set; }
        public ILULevelDto? ILULevel { get; set; }
    }
}
