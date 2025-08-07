using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionOperationSequenceDtos;

namespace SupervisorMobility.API.Models.SOS.SOSDistributionDtos
{
    public class SOSDistributionDto
    {
        public int SOSDistributionId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }

        public string? TackTime { get; set; }

        public ICollection<TurnDto>? Turns { get; set; }
        public string? AplicationModels { get; set; } = "§§§§";
        public ICollection<SOSDistributionOperationSequenceDto>? SOSDistributionOperationSequence { get; set; }

        public ICollection<SOSDistributionLogbookDto>? DistributionLogbooks { get; set; } = new List<SOSDistributionLogbookDto>();
        public ICollection<FileUploadGeneralDto>? Illustrations { get; set; } = new List<FileUploadGeneralDto>();
        public ICollection<CommentaryDto>? Notes { get; set; } = new List<CommentaryDto>();

        public string? AdditionalTime { get; set; } = "§§§§";
        public string? CycleTime { get; set; } = "§§§§";
        public string? ControlNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ApplicationMonth { get; set; }

        public int? SOSDistributionAdditionalTimeId { get; set; }
        public SOSDistributionAdditionalTime? SOSDistributionAdditionalTime { get; set; }

        public bool? IsActive { get; set; }

        public int? SOSHubId { get; set; }
        public ICollection<SOSHubDto>? SOSHubs { get; set; } = new List<SOSHubDto>();
        public ICollection<SOSAnalysisDto>? Analyses { get; set; } = new List<SOSAnalysisDto>();
        public ICollection<SOSSequenceDto>? Sequences { get; set; } = new List<SOSSequenceDto>();
    }
}
