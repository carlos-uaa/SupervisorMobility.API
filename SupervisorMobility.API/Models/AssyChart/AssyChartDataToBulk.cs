using DocumentFormat.OpenXml.Office.CoverPageProps;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.ProductDistributionsDtos;
using SupervisorMobility.API.Models.ProductDtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.AssyChart
{
    public class AssyChartDataToBulk
    {
        public int? AssyChardId { get; set; }
        public bool? IsActive { get; set; }
        public string? GOS { get; set; } = string.Empty;
        public string? CCP { get; set; } = string.Empty;
        public string? HOE { get; set; } = string.Empty;


        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }

        //Product info
        public int? ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductDescription { get; set; }
        public bool? ProductIsActive { get; set; }
        
        //PLANT INFO
        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }
        //AREA INFO
        public int? AreaId { get; set; }
        public string? AreaCode { get; set; }
        public string? AreaDescription { get; set; }    
        public bool? AreaIsActive { get; set; }
                
        //Distribution Info
        public int? DistributionId { get; set; }
        public string? DistributionCode { get; set; }
        public string? DistributionDescription { get; set; }
        public bool? DistributionIsActive { get; set; }

        //Operation Info
        public int? OperationId { get; set; }
        public string? OperationCode { get; set; }
        public string? OperationDescription { get; set;}
        public bool? OperationIsActive { get; set;}

   }
}
