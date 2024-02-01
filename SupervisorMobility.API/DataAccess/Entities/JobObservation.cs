using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.ILU;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities
{
    public class JobObservation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobObservationId { get; set; }
        public bool? IsActive { get; set; }

        //whit Info
        public Plant? Plant { get; set; }
        public int? PlantId { get; set; }
        public Area? Area { get; set; }
        public int? AreaId { get; set; }
        public Distribution? Distribution { get; set; }
        public int? DistributionId { get; set; }
        public Operation? Operation { get; set; }
        public int? OperationId { get; set; }
        //whit people
        public User? Supervisor { get; set; }
        public User? Operator { get; set; }
        public int? SupervisorId { get; set; }
        public int? OperatorId { get; set; }
        //whitLup
        public ICollection<Lup> Lup { get; set; } = new List<Lup>();
        //Whit History
        public ICollection<JobObservationVersion> History { get; set; } = new List<JobObservationVersion>();
        //Whit  answers to questions
        public ICollection<ChecklistAnswer>? checklistAnswers { get; set; } = new List<ChecklistAnswer>();

        public int? Type { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? FinishedDate { get; set; }

        public string? Justification { get; set; }
        public string? SectionIds { get; set; }
        public int? Status { get; set; }
        public int? Option { get; set; }
        public string? Anomaly { get; set; }

        public string? HOEStandardTimes { get; set; }
        public string? ModelsSpecification { get; set; }
        public string? Cycles { get; set; }

        public string? SsvCommentary { get; set; }
        public string? OperatorCommentary { get; set; }
        public string? SsvSignature { get; set; }
        public string? OperatorSignature { get; set; }

        

        public int? AssychartId { get; set; }
        public AssyChart? AssyChart { get; set; }

        public string? ReleasedFeedback { get; set; }

        public int? KpiId { get; set; }
        public string? TaktTime { get; set; }
        public string? Questions { get; set; }
        public int? ProductId { get; set; }

        public string? OperationTimesJson { get; set; }
        public string? StepsNumber { get; set; }
        public string? DoubleManagment { get; set; }
        public string? Waiting { get; set; }
    }
}
