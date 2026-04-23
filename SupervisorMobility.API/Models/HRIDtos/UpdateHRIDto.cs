using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

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
    }
}
