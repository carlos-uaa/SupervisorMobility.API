using Microsoft.EntityFrameworkCore.Infrastructure;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AssyChart;

namespace SupervisorMobility.API.Models.RouteProductAssyChartDtos
{
    public class RouteProductAssyChartWithOutNavigations
    {
        public string? GOS { get; set; } = string.Empty;
        public string? CCP { get; set; } = string.Empty;
        public string? HOE { get; set; } = string.Empty;

        public int AssyChardId { get; set; }

        public int? ProductId { get; set; }

        public bool? IsActive { get; set; }
    }
}
