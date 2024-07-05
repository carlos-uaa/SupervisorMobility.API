namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos
{
    public class DataPanelSpecificationForCreateDto
    {
        public bool? IsActive { get; set; }
        public int ItemOrder { get; set; }
        public string DataSpecification { get; set; } = string.Empty;
        public int? DataPanelId { get; set; }

    }
}
