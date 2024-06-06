using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.AppearanceDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.LogbookAppearanceDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.PartDtos;
using SupervisorMobility.API.Models.ProductDtos;

namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.LogbookAppearanceDtos
{
    public class LogbookAppearanceForUpdateDto
    {
        public int LogbookAppearanceId { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int? ApppearanceId { get; set; }
        public AppearanceDto? AppearanceInspection { get; set; }

        public ICollection<DataPanelAnswer>? PanelResults { get; set; }
          = new List<DataPanelAnswer>();
        public ICollection<ProblemDefectAnswer>? ProblemDefectResults { get; set; }
    = new List<ProblemDefectAnswer>();

        public string? Programmed { get; set; }

        public int? InspectorId { get; set; }
        public User? Inspector { get; set; }

        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
    }
}
