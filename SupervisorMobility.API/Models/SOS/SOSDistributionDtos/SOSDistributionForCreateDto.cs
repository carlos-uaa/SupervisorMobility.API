
using SupervisorMobility.API.Models.SOS.TurnDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionOperationSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;

namespace SupervisorMobility.API.Models.SOS.SOSDistributionDtos
{
    public class SOSDistributionForCreateDto
    {
        public int SOSDistributionId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }


        public string? TackTime { get; set; }

        public ICollection<TurnForCreateDto>? Turns { get; set; }
        public string? AplicationModels { get; set; } = "§§§§";

        public ICollection<SOSDistributionOperationSequenceForCreateDto>? SOSDistributionOperationSequence { get; set; }


        public ICollection<SOSDistributionLogbookForCreateDto>? DistributionLogbooks { get; set; } = new List<SOSDistributionLogbookForCreateDto>();
        public ICollection<FileUploadGeneralDto>? Illustrations { get; set; } = new List<FileUploadGeneralDto>();
        public ICollection<CreateCommentaryDto>? Notes { get; set; } = new List<CreateCommentaryDto>();

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
