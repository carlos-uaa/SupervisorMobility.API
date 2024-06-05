using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.ProductDtos;

namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointNormDtos
{
    public class CheckpointNormForCreateDto
    {
        public bool? IsActive { get; set; }

        //Formato tiene datos con tendencia a ser establecidos
        public int ItemOrder { get; set; }
        public string Standard { get; set; } = string.Empty;

        public int CheckpointId { get; set; }

    }
}
