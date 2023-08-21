using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class RouteProductAssyChart
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RouteProductAssyChartId { get; set; }

        public string? GOS { get; set; } = string.Empty;
        public string? CCP { get; set; } = string.Empty;
        public string? HOE { get; set; } = string.Empty;

        public int? AssyChardId { get; set; }
        public AssyChart? AssyChart { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public bool? IsActive { get; set; }

    }
}
