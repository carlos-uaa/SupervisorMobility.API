using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Models.HCICategoryDtos
{
    public class CreateHCICategoryDto
    {
        public int ChosenCategoryDepartmentId { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsActive { get; set; }
    }
}
