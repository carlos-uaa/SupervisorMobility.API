using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.GuidesDtos
{
    public class GuideWithoutFileDto
    {
        public int GuideId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public int FileUploadId { get; set; }

        public bool? IsActive { get; set; }
    }
}
