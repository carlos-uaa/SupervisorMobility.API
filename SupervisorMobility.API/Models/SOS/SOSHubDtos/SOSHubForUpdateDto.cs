using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;

namespace SupervisorMobility.API.Models.SOS.SOSHubDtos
{
    public class SOSHubForUpdateDto
    {
        public int SOSHubId { get; set; }
        public string OperationDescription { get; set; }
        public string ProcessSheet { get; set; }
        public ICollection<UpdateCommentaryDto>? ProcessSheetCommentary { get; set; } = new List<UpdateCommentaryDto>();
        
        public int? AppliedModelId { get; set; }


        public string RevisedItems { get; set; }

        public TimeSpan? TrainingTime { get; set; }
        public ICollection<EquipmentForUpdateDto>? SafetyEquipment { get; set; } = new List<EquipmentForUpdateDto>();
        public ICollection<ToolForUpdateDto>? ToolsUsed { get; set; } = new List<ToolForUpdateDto>();
        public ICollection<MaterialForUpdateDto>? MaterialsUsed { get; set; } = new List<MaterialForUpdateDto>();
        public string OtherInformation { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? DepartamentId { get; set; }

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
