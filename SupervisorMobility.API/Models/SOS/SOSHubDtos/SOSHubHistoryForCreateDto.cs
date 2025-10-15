using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisBkupDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOS.SOSHubDtos
{
    public class SOSHubHistoryForCreateDto
    {
        public int SOSHubId { get; set; }
        public string Folio { get; set; }
        public ICollection<AnalysisBkupDto> AnalysesBkup { get; set; } = new List<AnalysisBkupDto>();
        public ICollection<SectionDto> Sections { get; set; } = new List<SectionDto>();
        public string ProcessSheet { get; set; }
        public ICollection<CommentaryDto>? ProcessSheetCommentary { get; set; } = new List<CommentaryDto>();
        public int? AppliedModelId { get; set; }


        public string RevisedItems { get; set; }

        public int? TrainingTime { get; set; }
        public ICollection<EquipmentDto>? SafetyEquipment { get; set; } 
        public ICollection<ToolDto>? ToolsUsed { get; set; } 
        public ICollection<MaterialDto>? MaterialsUsed { get; set; } 
        public string OtherInformation { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? DepartmentId { get; set; }
        public int? StationId { get; set; }
        public int? CreatorId { get; set; }
        public User? Creator { get; set; }

        //public int? ApproverOwner { get; set; }
        public List<UsersWithoutNavigationWithoutPeopleDetails>? ApproversOwners { get; set; }

        public DateTime? CreatedDate { get; set; }

        public List<UsersWithoutNavigationWithoutPeopleDetails>? ReviewerEditors { get; set; }
        //public int? ReviewerEditorId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string Plan { get; set; }
        public string SourcePlan { get; set; }
        public string Status { get; set; }

        public string? VersionChanges { get; set; }


        public bool? IsActive { get; set; }
    }
}
