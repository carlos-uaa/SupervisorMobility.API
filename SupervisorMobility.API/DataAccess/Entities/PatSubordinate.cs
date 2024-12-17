using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class PatSubordinate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatSubordinateId { get; set; }
        public int PatId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}