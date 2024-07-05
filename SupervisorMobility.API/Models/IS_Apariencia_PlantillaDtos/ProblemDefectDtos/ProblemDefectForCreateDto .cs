namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.ProblemDefectDtos
{
    public class ProblemDefectForCreateDto
    {
        public bool? IsActive { get; set; }

        public int ItemOrder { get; set; }
        public string DefectDescription { get; set; } = string.Empty;
    }
}
