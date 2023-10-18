using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class FileUpload
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FileUploadId { get; set; }
        public string? FileName { get; set; }
        public string? StorageFileName { get; set; }
        public string? ContentType { get; set; }

        [Column(TypeName = "Date")]
        public DateTime UploadDate { get; set; }


    }
}
