using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.RouteProductAssyChartDtos;

namespace SupervisorMobility.API.Models.AssyChart
{
    public class AssyChartForUpdateDto
    {
        public int AssyChardId { get; set; }
        public bool? IsActive { get; set; }


        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; } = DateTime.Now;

        public ICollection<RouteProductAssyChartForUpdateDto> RoutesProductsAssyChart { get; set; } = new List<RouteProductAssyChartForUpdateDto>();


        //Navigation properties

        public int PlantId { get; set; }
        public int AreaId { get; set; }
        public int DistributionId { get; set; }
        public int OperationId { get; set; }

    }
}
