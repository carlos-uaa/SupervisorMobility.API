using SupervisorMobility.API.Models.RouteProductAssyChartDtos;

namespace SupervisorMobility.API.Models.AssyChart
{
    public class AssyChartForCreation
    {
        public bool? IsActive { get; set; }
      

        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime ModificationDate { get; set; }

        //Navigation properties
        public ICollection<RouteProductAssyChartForCreationDto> RoutesProductsAssyChart { get; set; }  = new List<RouteProductAssyChartForCreationDto>();

        //arbol
        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? DistributionId { get; set; }
        public int? OperationId { get; set; }
    }
}
