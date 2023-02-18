using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.OperationDtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.AssyChart
{
    public class AssyChartForUpdateDto
    {
        public int AssyChardId { get; set; }
        public bool? IsActive { get; set; }
        public string GOS { get; set; } = string.Empty;
        public string CCP { get; set; } = string.Empty;
        public string HOE { get; set; } = string.Empty;

        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; } = DateTime.Now;

        //Navigation properties
        public int ProductId { get; set; }
        public int PlantId { get; set; }
        public int AreaId { get; set; }
        public int DistributionId { get; set; }
        public int OperationId { get; set; }
        public OperationWithoutNavigationPropertiesDto Operation { get; set; } = new OperationWithoutNavigationPropertiesDto();

    }
}
