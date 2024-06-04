using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.ProductDtos;

namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.PartDtos
{
    public class AppearanceForUpdateDto
    {
        public int ApearanceId { get; set; }
        public bool? IsActive { get; set; }

        public int? PartId { get; set; }
        public PartDto? Part { get; set; }

        public ICollection<CommentaryDto>? Observations { get; set; }

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


        public ICollection<ProblemDefectDto>? ProblemDefectItems { get; set; }
         = new List<ProblemDefectDto>();

        public ICollection<LogbookAppearanceDto>? LogbooksAppearance { get; set; }
         = new List<LogbookAppearanceDto>();
    }
}
