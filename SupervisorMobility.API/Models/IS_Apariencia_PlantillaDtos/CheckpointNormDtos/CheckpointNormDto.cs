using SupervisorMobility.API.Models.FileUploadDto;

namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointNormDtos
{
    public class CheckpointNormDto
    {
        public int CheckpointNormId { get; set; }

        public bool? IsActive { get; set; }

        public int ItemOrder { get; set; }
        public string Standard { get; set; } = string.Empty;
        public int CheckpointId { get; set; }

        public ICollection<FileUploadGeneralDto>? Sketches { get; set; } = new List<FileUploadGeneralDto>();

    }
}
