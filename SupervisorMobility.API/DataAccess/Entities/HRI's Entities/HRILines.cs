using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities
{
    public class HRILines
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string LineName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
