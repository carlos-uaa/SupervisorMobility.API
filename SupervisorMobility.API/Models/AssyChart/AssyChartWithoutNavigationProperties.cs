using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.OperationDtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.AssyChart
{
    public class AssyChartWithoutNavigationProperties
    {
        public int AssyChardId { get; set; }
        public bool? IsActive { get; set; }
        public string GOS { get; set; } = string.Empty;
        public string CCP { get; set; } = string.Empty;
        public string HOE { get; set; } = string.Empty;

        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime ModificationDate { get; set; }
         
        //Navigation properties
        public int ProductId { get; set; }

        //arbol
        public int PlantId { get; set; }
        public int AreaId { get; set; }
        public int DistributionId { get; set; }
        public int OperationId { get; set; }
       
    }
}
