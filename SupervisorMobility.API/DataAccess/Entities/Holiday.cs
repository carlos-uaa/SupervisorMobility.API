using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class Holiday
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HolidayId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool IsNationalHoliday { get; set; } // opcional, por si luego necesitas distinguir
        public bool IsActive { get; set; } = true;
    }
}
