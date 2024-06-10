using Microsoft.Identity.Client;
using MimeKit.Encodings;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointDtos;

namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointNormDtos
{
    public class CheckpointNormDto
    {
        public int CheckpointNormId { get; set; }

        public bool? IsActive { get; set; }

        public int ItemOrder { get; set; }
        public string Standard { get; set; } = string.Empty;
        public int CheckpointId { get; set; }

        public CheckpointDto? Checkpoint { get; set; }

        public ICollection<FileUploadGeneralDto>? Sketches { get; set; } = new List<FileUploadGeneralDto>();

    }
}
