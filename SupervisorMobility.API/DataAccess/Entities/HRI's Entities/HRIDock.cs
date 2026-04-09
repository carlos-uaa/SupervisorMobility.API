using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities
{
    public class HRIDock
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DockName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
