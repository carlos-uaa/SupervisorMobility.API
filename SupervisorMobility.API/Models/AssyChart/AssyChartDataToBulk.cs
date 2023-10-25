using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.Models.AssyChart
{
    public class AssyChartDataToBulk
    {
        public int? AssyChardId { get; set; }
        public bool? IsActive { get; set; }
   

        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }

    
        //PLANT INFO
        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }
        //AREA INFO
        public int? AreaId { get; set; }
        public string? AreaCode { get; set; }
        public string? AreaDescription { get; set; }
        public bool? AreaIsActive { get; set; }

        //Distribution Info
        public int? DistributionId { get; set; }
        public string? DistributionCode { get; set; }
        public string? DistributionDescription { get; set; }
        public bool? DistributionIsActive { get; set; }

        //Operation Info
        public int? OperationId { get; set; }
        public string? OperationCode { get; set; }
        public string? OperationDescription { get; set; }
        public bool? OperationIsActive { get; set; }
        public int? ErgonomicsLevel { get; set; }

    }
}
