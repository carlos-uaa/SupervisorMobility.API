using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.OperationDtos
{
    public class OperationForCreationDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        public string? restrictionorcomment { get; set; } = string.Empty;
        public string? jsonTimeProduct { get; set; } = string.Empty;
        public string? ProductName { get; set; }
        public string? NameTime { get; set; }
        public string? Time { get; set; } 
        public string? AdditionalTime { get; set; }
        public string? StandardTime { get; set; } 
        public int CriticalType { get; set; }

        public bool IsActive { get; set; }
    }
}
