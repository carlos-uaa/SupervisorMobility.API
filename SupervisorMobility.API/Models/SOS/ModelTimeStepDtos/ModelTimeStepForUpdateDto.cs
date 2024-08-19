namespace SupervisorMobility.API.Models.SOS.ModelTimeStepDtos
{
    public class ModelTimeStepForUpdateDto
    {
        public int ModelTimeStepId { get; set; }

        public int SectionId { get; set; }
        public string? Times { get; set; } = "§§§§";
    }
}
