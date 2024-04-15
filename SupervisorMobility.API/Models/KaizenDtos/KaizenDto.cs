using SupervisorMobility.API.Models.FileUploadDto;

namespace SupervisorMobility.API.Models.KaizenDtos
{
    public class KaizenDto
    {
        public int KaizenId { get; set; }
        public string KaizenName { get; set; }

        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }

        public int? AreaId { get; set; }

        public int PillarId { get; set; }

        public int? SupervisorId { get; set; }

        public int? SeniorSupervisorId { get; set; }

        public int? ProposedId { get; set; }

     

        public string PreviousJustification { get; set; }

        public string ThenJustification { get; set; }

        public string StandardModification { get; set; }

        public string CalculationFormula { get; set; }

        public double? Total { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? FinishedDate { get; set; }
        public int Status { get; set; }
        public string kpiName { get; set; }
        public bool IsSignedSSV { get; set; }
        public bool IsSignedSupervisor { get; set; }
        public int OperatorPayroll { get; set; }
    }
}
