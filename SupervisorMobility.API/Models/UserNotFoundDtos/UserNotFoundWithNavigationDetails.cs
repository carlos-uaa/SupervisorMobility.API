using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.GroupDtos;
using SupervisorMobility.API.Models.ILURegisterDtos;
using SupervisorMobility.API.Models.PlantDtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.Users
{
    public class UserNotFoundWithNavigationDetails
    {
        public int UserNotFoundId { get; set; }
        public string? ObjectId { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }

    }
}
