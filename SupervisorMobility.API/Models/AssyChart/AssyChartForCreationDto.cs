using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.AssyChart
{
    public class AssyChartForCreationDto
    {
        public bool? IsActive { get; set; }
        public string GOS { get; set; } = string.Empty;
        public string CCP { get; set; } = string.Empty;
        public string HOE { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }

        //Navigation properties
        public int ProductId { get; set; }

        //arbol
        public int PlantId { get; set; }

        public int AreaId { get; set; }

        public int DistributionId { get; set; }

        //Data Operation to create
        [Required]
        [MaxLength(50)]
        public string CodeOperation { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string DescriptionOperation { get; set; } = string.Empty;
        public bool IsActiveOperation { get; set; }

    }
}
