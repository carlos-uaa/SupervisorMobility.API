using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos
{
    public class DataPanelSpecificationForUpdateSequenceDto
    {
        [Required]
        public int ItemOrder { get; set; }
    }
}
