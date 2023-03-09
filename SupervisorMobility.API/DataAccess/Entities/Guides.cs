using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class Guides
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GuideId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    
        public int FileUploadId { get; set; }
        public FileUpload? FileUpload { get; set; }

    }
}
