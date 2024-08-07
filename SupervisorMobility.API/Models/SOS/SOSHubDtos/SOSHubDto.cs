using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.DepartmentDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.ProductDtos;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisBkupDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.StationDtos;
using SupervisorMobility.API.Models.SOS.ToolsUsedDtos;

namespace SupervisorMobility.API.Models.SOS.SOSHubDtos
{
    public class SOSHubDto
    {
        public int SOSHubHistoryId { get; set; }

        public int SOSHubId { get; set; }
        public string? Folio { get; set; }
        public ICollection<AnalysisBkupDto> AnalysesBkup { get; set; } = new List<AnalysisBkupDto>();
        public ICollection<SectionDto> Sections { get; set; } = new List<SectionDto>();
        public string ProcessSheet { get; set; }
        public ICollection<CommentaryDto>? ProcessSheetCommentary { get; set; } = new List<CommentaryDto>();
        public ICollection<CommonDirectionDto>? CommonDirection { get; set; } = new List<CommonDirectionDto>();
        public int? AppliedModelId { get; set; }
        public ProductDto? AppliedModel { get; set; }


        public ICollection<FileUploadGeneralDto>? Images { get; set; } = new List<FileUploadGeneralDto>();
        public ICollection<FileUploadGeneralDto>? Videos { get; set; } = new List<FileUploadGeneralDto>();
        public string RevisedItems { get; set; }

        public string? TrainingTime { get; set; }
        public ICollection<EquipmentDto>? SafetyEquipment { get; set; } = new List<EquipmentDto>();
        public ICollection<ToolUsedDto>? ToolsUsed { get; set; } = new List<ToolUsedDto>();
        public ICollection<MaterialsUsedDto>? MaterialsUsed { get; set; } = new List<MaterialsUsedDto>();
        public string OtherInformation { get; set; }

        public int? PlantId { get; set; }
        public PlantDto? Plant { get; set; }
        public int? AreaId { get; set; }
        public AreaWithoutNavigationPropertiesDto? Area { get; set; }
        public int? DistributionId { get; set; }
        public DistributionWithoutNavigationPropertiesDto? Distribution { get; set; }
        public int? DepartmentId { get; set; }
        public DepartmentDto? Department { get; set; }
        public int? StationId { get; set; }
        public StationDto? Station { get; set; }
        public int? OwnerId { get; set; }
        public UsersWithoutPeopleWithNavigation? Owner { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? EditorId { get; set; }
        public UsersWithoutPeopleWithNavigation? Editor { get; set; }

        public DateTime? ModifiedDate { get; set; }


        //estos 3 podrian ser una entidad (pero la flojera)
        public string Plan { get; set; }
        public string SourcePlan { get; set; }
        public string Status { get; set; }

        public string? VersionChanges { get; set; }

        public bool? IsActive { get; set; }
    }
}
