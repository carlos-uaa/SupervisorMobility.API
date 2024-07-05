using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;

namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos
{
    public class DataPanelForCreateDto
    {
        public bool? IsActive { get; set; }

        //Formato tiene datos con tendencia a ser establecidos
        public int ItemOrder { get; set; }
        public string DataTitle { get; set; } = string.Empty;
        public ICollection<DataPanelSpecificationForCreateDto>? Specifications { get; set; }
    }
}
