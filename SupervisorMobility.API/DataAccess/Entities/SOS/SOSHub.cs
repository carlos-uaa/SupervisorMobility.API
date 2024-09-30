using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSHub
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSHubId { get; set; }

        public string? Folio { get; set; }

        //Es el analisis
        public ICollection<AnalysisBkup>? AnalysesBkup { get; set; } = new List<AnalysisBkup>();
        public ICollection<Section>? Sections { get; set; } = new List<Section>();
        public string? ProcessSheet { get; set; }
        public ICollection<Commentary>? ProcessSheetCommentary { get; set; } = new List<Commentary>();
        public ICollection<CommonDirection>? CommonDirection { get; set; } = new List<CommonDirection>();


        public ICollection<Product>? AppliedModels { get; set; } = new List<Product>();
       
     
     

        public ICollection<FileUpload>? Images { get; set; } = new List<FileUpload>();
        public ICollection<FileUpload>? Videos { get; set; } = new List<FileUpload>();
        public string? RevisedItems { get; set; }

        public string? TrainingTime { get; set; }
        public ICollection<Equipment>? SafetyEquipment { get; set; } = new List<Equipment>();
        public ICollection<ToolUsed>? ToolsUsed { get; set; } = new List<ToolUsed>();
        public ICollection<MaterialUsed>? MaterialsUsed { get; set; } = new List<MaterialUsed>();
        public string? OtherInformation { get; set; }

        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }
        public int? AreaId { get; set; }
        public Area? Area { get; set; }
        public int? DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department{ get; set; }
        public int? StationId { get; set; }
        public Station? Station { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public ICollection<User>? ApproverOwners { get; set; } = new List<User>();
        public ICollection<User>? ReviewerEditors { get; set; } = new List<User>();


        public string? Plan { get; set; } 
        public string? SourcePlan { get; set; }
        public string? Status { get; set; }


        public ICollection<SOSAnalysis>? SOSAnalysis { get; set; } = new List<SOSAnalysis>();
        public ICollection<SOSCombination>? SOSCombination { get; set; } = new List<SOSCombination>();
        public ICollection<SOSDistribution>? SOSDistribution { get; set; } = new List<SOSDistribution>();
        public ICollection<SOSFlow>? SOSFlow { get; set; } = new List<SOSFlow>();
        public ICollection<SOSSequence>? SOSSequence { get; set; } = new List<SOSSequence>();

        public ICollection<SOSHubHistory>? History { get; set; } = new List<SOSHubHistory>();

        public bool? IsActive { get; set; }
    }
}
