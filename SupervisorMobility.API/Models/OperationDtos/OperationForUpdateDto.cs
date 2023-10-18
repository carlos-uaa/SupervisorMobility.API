using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.OperationDtos
{
    public class OperationForUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        public int CriticalType { get; set; }

        public bool IsActive { get; set; }
    }
}
