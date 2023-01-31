using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.ProductOperationDtos
{
    public class ProductOperationForCreationDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
