using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class HCICategory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HCICategoryId { get; set; }

        public int? ChosenCategoryDepartmentId { get; set; }
        public Department? ChosenCategory { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsActive { get; set; }
    }
}