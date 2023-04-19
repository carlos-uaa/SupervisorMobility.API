using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class Attendance
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttendanceId { get; set; }

        //Supervisor
        public int? SuperiorId { get; set; }
        public User? Superior { get; set; }
        //Info de Usuario
        public int? UserId { get; set; }
        public User? User { get; set; }

        //Distribucion actual assignada a cubrir en el dia
        public int? CurrentdistributionId { get; set; }
        public Distribution? currentdistribution { get; set; }

        public bool Compas { get; set; }
        public bool Station { get; set; }
    }
}
