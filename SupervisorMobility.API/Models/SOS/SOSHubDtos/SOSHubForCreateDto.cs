using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisBkupDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;

namespace SupervisorMobility.API.Models.SOS.SOSHubDtos
{
    public class SOSHubForCreateDto
    {
        public ICollection<AnalysisBkupForCreateDto> AnalysesBkup { get; set; } = new List<AnalysisBkupForCreateDto>();
        public ICollection<SectionForCreateDto> Sections { get; set; } = new List<SectionForCreateDto>();
        public string ProcessSheet { get; set; }
        public ICollection<CreateCommentaryDto>? ProcessSheetCommentary { get; set; } = new List<CreateCommentaryDto>();
        
        public int? AppliedModelId { get; set; }


        public string RevisedItems { get; set; }

        public string? TrainingTime { get; set; }
        public ICollection<EquipmentDto>? SafetyEquipment { get; set; } = new List<EquipmentDto>();
        public ICollection<ToolDto>? ToolsUsed { get; set; } = new List<ToolDto>();
        public ICollection<MaterialDto>? MaterialsUsed { get; set; } = new List<MaterialDto>();
        public string OtherInformation { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? DistributionId { get; set; }
        public int? DepartmentId { get; set; }

        public int? OwnerId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? EditorId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string Plan { get; set; }
        public string SourcePlan { get; set; }
        public string Status { get; set; }


        public bool? IsActive { get; set; }
    }
}
