using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.HCICategoryDtos
{
    public class HCICategoryDto
    {
        public int HCICategoryId { get; set; }
        public int ChosenCategoryDepartmentId { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsActive { get; set; }
    }
}
