
namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointNormDtos
{
    public class CheckpointNormForUpdateDto
    {
        public int CheckpointNormId { get; set; }

        public bool? IsActive { get; set; }

        //Formato tiene datos con tendencia a ser establecidos
        public int ItemOrder { get; set; }
        public string Standard { get; set; } = string.Empty;
        public int CheckpointId { get; set; }


    }
}
