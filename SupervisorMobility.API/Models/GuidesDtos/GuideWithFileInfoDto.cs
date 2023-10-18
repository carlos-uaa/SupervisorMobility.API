using SupervisorMobility.API.Models.FileUploadDto;

namespace SupervisorMobility.API.Models.GuidesDtos
{
    public class GuideWithFileInfoDto
    {
        public int GuideId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public int FileUploadId { get; set; }
        public FileUploadGeneralDto? FileUpload { get; set; } = new FileUploadGeneralDto();
    }
}
