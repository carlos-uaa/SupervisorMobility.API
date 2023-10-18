using SupervisorMobility.API.Models.FileUploadDto;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.GuidesDtos
{
    public class GuideForCreationDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }

        public int FileUploadId { get; set; }
        public FileUploadGeneralDto File { get; set; } = new FileUploadGeneralDto();

    }
}
