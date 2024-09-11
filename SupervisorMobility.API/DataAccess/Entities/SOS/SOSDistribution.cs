using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class SOSDistribution
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOSDistributionId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }

    
        public string? TackTime {  get; set; }

        public ICollection<Turn>? Turns { get; set; }
        public string? AplicationModels { get; set; } = "§§§§";
        public ICollection<SOSTime>? Times { get; set; } = new List<SOSTime>();


        public string? AdditionalTime { get; set; } = "§§§§";
        public string? CycleTime { get; set; } = "§§§§";
        public string? ControlNumber { get; set; }
        public ICollection<SOSDistributionLogbook>? DistributionLogbooks { get; set; } = new List<SOSDistributionLogbook>();
        public ICollection<FileUpload>? Illustrations { get; set; } = new List<FileUpload>();
        public ICollection<Commentary>? Notes { get; set; } = new List<Commentary>();
        public DateTime? CreatedAt { get; set; }
        public DateTime? ApplicationMonth { get; set; }

        public bool? IsActive { get; set; }
        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
    }
}
