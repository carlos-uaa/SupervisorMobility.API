using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.ProductDtos;

namespace SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.PartDtos
{
    public class PartForUpdateDto
    {
        public int PartId { get; set; }
        public bool? IsActive { get; set; }

        public string? PartName { get; set; } = string.Empty;
        public string? PartNumber { get; set; } = string.Empty;

        public int ModelId { get; set; }

        public ICollection<FileUploadGeneralDto> Sketches { get; set; } = new List<FileUploadGeneralDto>();
    }
}
