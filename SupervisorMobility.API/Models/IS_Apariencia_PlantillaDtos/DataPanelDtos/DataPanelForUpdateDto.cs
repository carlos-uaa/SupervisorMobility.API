using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;

namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos
{
    public class DataPanelForUpdateDto
    {
        public int DataPanelId { get; set; }

        public bool? IsActive { get; set; }

        
        public int ItemOrder { get; set; }
        public string? DataTitle { get; set; }
        public ICollection<DataPanelSpecificationForUpdateDto>? Specifications { get; set; }

    }
}
