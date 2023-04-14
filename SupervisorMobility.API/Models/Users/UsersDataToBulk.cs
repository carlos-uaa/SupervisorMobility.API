using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.GroupDtos;
using SupervisorMobility.API.Models.PlantDtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.Users
{
    public class UsersDataToBulk
    {
        public int UserId { get; set; }
        public string? ObjectId { get; set; }
        public int? Payroll { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UserType { get; set; }


        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        public string? PlantCode { get; set; }
        public string? PlantDescription { get; set; }
        public bool? PlantIsActive { get; set; }

        //AREA INFO
        public int? AreaId { get; set; }
        public string? AreaCode { get; set; }
        public string? AreaDescription { get; set; }
        public bool? AreaIsActive { get; set; }
        //grupo info
        public int? GroupId { get; set; }
        public string? GroupCode { get; set;}
        public string? GroupDescription { get; set;}
        public bool? GroupIsActiv { get; set;}

        public DateTime? CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public DateTime? DisabledDate { get; set; }

    }
}
