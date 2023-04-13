using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class Attendance
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttendanceId { get; set; }
        public string? Name { get; set; }
        public int? Payroll { get; set; }
        public int? AreaId { get; set; }
        public Area? Area { get; set; }
        public int? GroupId { get; set; }
        public Group? Group { get; set; }
        public bool Compas { get; set; }
        public bool Station { get; set; }
    }
}
