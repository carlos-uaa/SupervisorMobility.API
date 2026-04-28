namespace SupervisorMobility.API.Models.HRIDtos.HRImagesDto
{
    public class HRImageContentDto
    {
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
    }
}
