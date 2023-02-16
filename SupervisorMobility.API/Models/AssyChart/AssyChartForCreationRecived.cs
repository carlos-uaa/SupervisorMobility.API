using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.AssyChart
{
    public class AssyChartForCreationRecived
    {
        public string GOS { get; set; } = string.Empty;
        public string CCP { get; set; } = string.Empty;
        public string HOE { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime ModificationDate { get; set; } = DateTime.Now;

        //Navigation properties
        public int ProductId { get; set; }
        //arbol
        public int PlantId { get; set; }
        public int AreaId { get; set; }
        public int DistributionId { get; set; }
        //Data Operation to create
        [Required]
        [MaxLength(50)]
        public string OperationCode { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string OperationDescription { get; set; } = string.Empty;
        public bool OperationIsActive { get; set; }

    }
}
