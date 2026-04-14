using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities
{
    public class HRIItem
    {
        [Key]
        public int Id { get; set; }
        public string ControlNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public List<HRI>? HRIs { get; set; }
    }
}
