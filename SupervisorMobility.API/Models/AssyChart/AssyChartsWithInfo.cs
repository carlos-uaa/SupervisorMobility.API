using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.ProductDtos;
using SupervisorMobility.API.Models.RouteProductAssyChartDtos;

namespace SupervisorMobility.API.Models.AssyChart
{
    public class AssyChartWhitInfo
    {
        public int AssyChardId { get; set; }
        public bool? IsActive { get; set; }
       

        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }

        //Routes
        public ICollection<RouteProductAssyChartWithNavigations> RoutesProductsAssyChart { get; set; } = new List<RouteProductAssyChartWithNavigations>();


        //PLANT INFO
        public int PlantId { get; set; }
        public PlantDto Plant { get; set; } = new PlantDto();
        //AREA INFO
        public int AreaId { get; set; }
        public AreaWithoutNavigationPropertiesDto? Area { get; set; } = new AreaWithoutNavigationPropertiesDto();
        //Distribution Info
        public int DistributionId { get; set; }
        public DistributionWithNavigationPropertiesDto Distribution { get; set; } = new DistributionWithNavigationPropertiesDto();
        //Operation Info
        public int OperationId { get; set; }
        public OperationWithoutNavigationPropertiesDto Operation { get; set; } = new OperationWithoutNavigationPropertiesDto();
        public int? ErgonomicsLevel { get; set; }
    }
}
