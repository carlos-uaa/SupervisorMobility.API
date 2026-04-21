
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.HRICyclesDtos;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.HRIDtos
{
    public class GetHRIDto
    {
        public int HriId { get; set; }
        public int? HRILinesId { get; set; }
        public HRILines? Line { get; set; }
        public int? HRIItemId { get; set; }
        public HRIItem? NameOfItem { get; set; }
        public string? ControlNumber { get; set; }
        public int? HRIDockId { get; set; }
        public HRIDock? Dock { get; set; }
        public string? Department { get; set; }
        public List<HRImages>? Images { get; set; }
        public List<GetHRIRevisionItemDto>? ItemsRevised { get; set; }
        public List<WeeklyRevisions>? WeeklyRevisions { get; set; }
        public List<GetHRICyclesDto>? HriCycles { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public GetHourmeterRevisionDto? HourmeterRevision { get; set; }
        public int? SupervisorUserId { get; set; }
        public GetUserForHRIDailyRevsionDto? Supervisor { get; set; }
        public int? SSVUserId { get; set; }
        public GetUserForHRIDailyRevsionDto? SSV { get; set; }

        public int? PlantId { get; set; }
        public GetPlantForHRIDto? Plant { get; set; }
        public int? AreaId { get; set; }
        public GetAreaForHRIDto? Area { get; set; }
    }
}
