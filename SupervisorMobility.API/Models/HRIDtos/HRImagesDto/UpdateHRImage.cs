using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.Models.HRIDtos.HRImagesDto
{
    public class UpdateHRImageDto
    {
        public int? HriId { get; set; }
        public string ImageUrl { get; set; }
        public string ImageType { get; set; }
    }
}
