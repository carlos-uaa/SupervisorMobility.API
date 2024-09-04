using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisBkupDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using SupervisorMobility.API.Models.SOS.ToolsUsedDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.SOSHubDtos
{
    public class SOSHubForCreateDto
    {
        public string Folio { get; set; }
        public ICollection<AnalysisBkupForCreateDto> AnalysesBkup { get; set; } = new List<AnalysisBkupForCreateDto>();
        public ICollection<SectionForCreateDto> Sections { get; set; } = new List<SectionForCreateDto>();
        public string ProcessSheet { get; set; }
        public ICollection<CreateCommentaryDto>? ProcessSheetCommentary { get; set; } = new List<CreateCommentaryDto>();
        public ICollection<CommonDirectionDto>? CommonDirection { get; set; } = new List<CommonDirectionDto>();

        public int? AppliedModelId { get; set; }


        public string RevisedItems { get; set; }

        public string? TrainingTime { get; set; }
        public ICollection<EquipmentDto>? SafetyEquipment { get; set; } = new List<EquipmentDto>();
        public ICollection<ToolUsedForCreateDto>? ToolsUsed { get; set; } = new List<ToolUsedForCreateDto>();
        public ICollection<MaterialsUsedForCreateDto>? MaterialsUsed { get; set; } = new List<MaterialsUsedForCreateDto>();
        public string OtherInformation { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? DistributionId { get; set; }
        public int? DepartmentId { get; set; }
        public int? StationId { get; set; }

       //public int? ApproverOwnerId { get; set; }

        public DateTime? CreatedDate { get; set; }

        //public int? ReviewerEditorId { get; set; }
        public List<UsersWithoutPeopleWithNavigation>? ApproverOwners { get; set; }
        public List<UsersWithoutPeopleWithNavigation>? ReviewerEditors { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string Plan { get; set; }
        public string SourcePlan { get; set; }
        public string Status { get; set; }


        public bool? IsActive { get; set; }
    }
}
