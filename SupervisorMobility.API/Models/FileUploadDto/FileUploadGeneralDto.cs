namespace SupervisorMobility.API.Models.FileUploadDto
{
    public class FileUploadGeneralDto
    {
        public int FileUploadId { get; set; }
        public string? FileName { get; set; }
        public string? StorageFileName { get; set; }
        public string? ContentType { get; set; }
        public DateTime UploadDate { get; set; }
        public bool? IsActive { get; set; }

    }
}
