using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;

namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos
{
    public class DataPanelDto
    {
        public int DataPanelId { get; set; }

        public bool? IsActive { get; set; }

        //Formato tiene datos con tendencia a ser establecidos
        public int ItemOrder { get; set; }
        public string DataTitle { get; set; } = string.Empty;

        public ICollection<DataPanelSpecificationDto>? Specifications { get; set; }
        = new List<DataPanelSpecificationDto>();
    }
}
