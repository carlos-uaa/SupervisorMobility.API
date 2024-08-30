using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;

namespace SupervisorMobility.API.DataAccess.Entities.SOS
{
    public class Material
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaterialId { get; set; }
        public string? key { get; set; }
        public string? PartName { get; set; }
        public string? PartNumber { get; set; }
        public bool? IsActive { get; set; }
    }
}
