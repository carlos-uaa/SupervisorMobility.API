using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.LogbookAppearanceDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.PartDtos;

namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.AppearanceDtos
{
    public class AppearanceForUpdateDto
    {
        public int ApearanceId { get; set; }
        public bool? IsActive { get; set; }

        public int? PartId { get; set; }
        public PartDto? Part { get; set; }

        public ICollection<UpdateCommentaryDto>? Observations { get; set; } = new List<UpdateCommentaryDto>();

        public int? ManufacturerId { get; set; }
        public User? Manufacturer { get; set; }
        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public int? ApproverUserId { get; set; }
        public User? ApproverUser { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? CheckDate { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public ICollection<DataPanelDto>? DataPanelItems { get; set; }
         = new List<DataPanelDto>();


        public ICollection<ProblemDefect>? ProblemDefectItems { get; set; }
         = new List<ProblemDefect>();

        public ICollection<LogbookAppearanceForUpdateDto>? LogbooksAppearance { get; set; }
         = new List<LogbookAppearanceForUpdateDto>();
    }
}
