using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointNormDtos;
using SupervisorMobility.API.Models.ProductDtos;

namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointDtos
{
    public class CheckpointForUpdateDto
    {
        public int CheckpointId { get; set; }

        public bool? IsActive { get; set; }

        //Formato tiene datos con tendencia a ser establecidos
        public int ItemOrder { get; set; }
        public string CheckpointTitle { get; set; } = string.Empty;
        public string CheckpointDescription { get; set; } = string.Empty;

        public ICollection<FileUploadGeneralDto>? Sketches { get; set; } = new List<FileUploadGeneralDto>();
        public ICollection<CheckpointNormForUpdateDto>? Standars { get; set; } = new List<CheckpointNormForUpdateDto>();
    }
}
