using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.Models.HRIDtos
{
    public class GetHRIToTableDto
    {
        public int HriId { get; set; }
        public HRILines? Line { get; set; }
        public HRIItem? NameOfItem { get; set; }
        public string? ControlNumber { get; set; }
        public string? Department { get; set; }
        public int ImagesCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
