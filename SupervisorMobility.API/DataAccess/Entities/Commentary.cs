using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class Commentary
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ComentaryId { get; set; }
       
        public string? Comment { get; set; }
        
        public bool? IsActive { get; set; }
    }
}
