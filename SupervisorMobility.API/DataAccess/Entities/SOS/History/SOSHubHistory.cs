using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.DataAccess.Entities.SOS.History
{
    public class SOSHubHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSHubHistoryId { get; set; }
        public int SOSHubId { get; set; }

        public string? Folio { get; set; }

        //Es el analisis
        public ICollection<AnalysisBkupHistory>? AnalysesBkup { get; set; } = new List<AnalysisBkupHistory>();
        public ICollection<SectionHistory>? Sections { get; set; } = new List<SectionHistory>();

        public string ProcessSheet { get; set; }
        public ICollection<CommentaryHistory>? ProcessSheetCommentary { get; set; } = new List<CommentaryHistory>();
        public ICollection<CommonDirection>? CommonDirection { get; set; } = new List<CommonDirection>();
        public int? AppliedModelId { get; set; }
        public Product? AppliedModel { get; set; }

        public ICollection<FileUpload>? Images { get; set; } = new List<FileUpload>();
        public ICollection<FileUpload>? Videos { get; set; } = new List<FileUpload>();
        public string RevisedItems { get; set; }

        public string? TrainingTime { get; set; }
        public ICollection<Equipment>? SafetyEquipment { get; set; } 
        public ICollection<Tool>? ToolsUsed { get; set; } 
        public ICollection<Material>? MaterialsUsed { get; set; }
        public string OtherInformation { get; set; }

        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }
        public int? AreaId { get; set; }
        public Area? Area { get; set; }
        public int? DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? StationId { get; set; }
        public Station? Station { get; set; }

        public int? OwnerId { get; set; }
        public User? Owner { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? CreatedDate { get; set; }

        public int? EditorId { get; set; }
        public User? Editor { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? ModifiedDate { get; set; }


        //estos 3 podrian ser una entidad (pero la flojera)
        public string Plan { get; set; }
        public string SourcePlan { get; set; }
        public string Status { get; set; }


        //public ICollection<SOSAnalysis>? SOSAnalysis { get; set; } = new List<SOSAnalysis>();
        //public ICollection<SOSCombination>? SOSCombination { get; set; } = new List<SOSCombination>();
        //public ICollection<SOSDistribution>? SOSDistribution { get; set; } = new List<SOSDistribution>();
        //public ICollection<SOSFlow>? SOSFlow { get; set; } = new List<SOSFlow>();
        //public ICollection<SOSSequence>? SOSSequence { get; set; } = new List<SOSSequence>();

        public string? VersionChanges { get; set; }
        public bool? IsActive { get; set; }
    }
}
