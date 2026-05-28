using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class LocalUserCourses
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseId { get; set; }
        public string Reticulate {  get; set; } = string.Empty;
        public DateTime Date { get; set; } = new DateTime();
        public decimal Calification { get; set; } = 0;
        public string Type { get; set; } = string.Empty;
        public int HCIId { get; set; }
        public HCI? HCI { get; set; }
    }
}
