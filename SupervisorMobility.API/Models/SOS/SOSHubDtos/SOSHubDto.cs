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

namespace SupervisorMobility.API.Models.SOS.SOSHubDtos
{
    public class SOSHubDto
    {
        public int SOSHubId { get; set; }
        public string OperationDescription { get; set; }
        public string ProcessSheet { get; set; }
        public ICollection<CommentaryDto>? ProcessSheetCommentary { get; set; } = new List<CommentaryDto>();
        public ICollection<FileUploadGeneralDto>? CommonDirection { get; set; } = new List<FileUploadGeneralDto>();
        public int? AppliedModelId { get; set; }
        public ProductDto? AppliedModel { get; set; }


        public ICollection<FileUploadGeneralDto>? Images { get; set; } = new List<FileUploadGeneralDto>();
        public ICollection<FileUploadGeneralDto>? Videos { get; set; } = new List<FileUploadGeneralDto>();
        public string RevisedItems { get; set; }

        public TimeSpan? TrainingTime { get; set; }
        public ICollection<EquipmentDto>? SafetyEquipment { get; set; } = new List<EquipmentDto>();
        public ICollection<ToolDto>? ToolsUsed { get; set; } = new List<ToolDto>();
        public ICollection<MaterialDto>? MaterialsUsed { get; set; } = new List<MaterialDto>();
        public string OtherInformation { get; set; }

        public int? PlantId { get; set; }
        public PlantDto? Plant { get; set; }
        public int? AreaId { get; set; }
        public AreaWithoutNavigationPropertiesDto? Area { get; set; }
        public int? DepartamentId { get; set; }
        public DepartmentDto? Department { get; set; }

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


        public bool? IsActive { get; set; }
    }
}
