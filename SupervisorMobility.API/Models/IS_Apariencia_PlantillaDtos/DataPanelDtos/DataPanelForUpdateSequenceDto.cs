using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos
{
    public class DataPanelForUpdateSequenceDto
    {
        [Required]
        public int ItemOrder { get; set; }
    }
}
