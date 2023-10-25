
using SupervisorMobility.API.Models.RouteProductAssyChartDtos;

namespace SupervisorMobility.API.Models.AssyChart
{
    public class AssyChartWithoutNavigationProperties
    {
        public int AssyChardId { get; set; }
        public bool? IsActive { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime ModificationDate { get; set; }

        //Navigation properties
        public ICollection<RouteProductAssyChartWithOutNavigations> RoutesProductsAssyChart { get; set; } = new List<RouteProductAssyChartWithOutNavigations>();


        //arbol
        public int PlantId { get; set; }
        public int AreaId { get; set; }
        public int DistributionId { get; set; }
        public int OperationId { get; set; }
        public int? ErgonomicsLevel { get; set; }

    }
}
