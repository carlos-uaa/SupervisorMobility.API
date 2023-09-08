using Microsoft.EntityFrameworkCore.Infrastructure;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.ProductDtos;

namespace SupervisorMobility.API.Models.RouteProductAssyChartDtos
{
    public class RouteProductAssyChartWithOutNavigations
    {
        public int SOSCodePathId { get; set; }

        public string Code { get; set; } = string.Empty;

        public string? GOS { get; set; } = string.Empty;
        public string? CommonDirectionGOS { get; set; } = string.Empty;
        public string? CCP { get; set; } = string.Empty;
        public string? CommonDirectionCCP { get; set; } = string.Empty;
        public string? HOE { get; set; } = string.Empty;
        public string? CommonDirectionHOE { get; set; } = string.Empty;

        public int DistributionId { get; set; }

        public int? ProductId { get; set; }
        public ProductDtos.ProductDto? Product { get; set; }

        public int? AssyChardId { get; set; }

        public bool? IsActive { get; set; }
    }
}
