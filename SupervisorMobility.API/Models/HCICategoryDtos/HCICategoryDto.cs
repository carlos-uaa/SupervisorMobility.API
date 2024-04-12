using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.HCICategoryDtos
{
    public class HCICategoryDto
    {
        public int HCICategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsActive { get; set; }
    }
}
