using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.Models.HRICyclesDtos;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;

namespace SupervisorMobility.API.Models.HRIDtos
{
    public class UpdateHRIDto
    {
        public int? HRILinesId { get; set; }       
        public int? HRIItemId { get; set; }
        public string? ControlNumber { get; set; }
        public int? HRIDockId { get; set; }
        public string? Department { get; set; }
        public int? SupervisorUserId { get; set; }
        public int? SSVUserId { get; set; }
        public List<UpdateRevisionItemDto>? RevisionItems { get; set; }
        public List<UpdateFullHRICyclesDto>? HRICycles { get; set; }
    }
}
