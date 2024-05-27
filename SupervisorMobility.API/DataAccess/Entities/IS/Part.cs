using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.DataAccess.Entities.Paths;
using Microsoft.Identity.Client;

namespace SupervisorMobility.API.DataAccess.Entities.IS
{
    public class Part
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PartId { get; set; }
        public bool? IsActive { get; set; }

        public string PartName { get; set; } = string.Empty;
        public int PartNumber { get; set; }

        public int ModelId { get; set; }
        public Product? Model { get; set; }

        public ICollection<FileUpload> Sketches { get; set; } = new List<FileUpload>();

    }
}
