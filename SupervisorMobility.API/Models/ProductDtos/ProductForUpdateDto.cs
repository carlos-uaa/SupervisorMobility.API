using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.ProductDtos
{
    public class ProductForUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
