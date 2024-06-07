namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.ProblemDefectDtos
{
    public class ProblemDefectDto
    {
        public int ProblemDefectId { get; set; }

        public bool? IsActive { get; set; }

        public int ItemOrder { get; set; }
        public string DefectDescription { get; set; } = string.Empty;

    }
}
