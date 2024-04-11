using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.FileUploadDto
{
    public class FileUploadForCreationDto
    {
        [Required]
        public string? FileName { get; set; }
        [Required]
        public string? StorageFileName { get; set; }
        [Required]
        public string? ContentType { get; set; }
        public DateTime UploadDate { get; set; }
        public bool? IsActive { get; set; }

    }
}
