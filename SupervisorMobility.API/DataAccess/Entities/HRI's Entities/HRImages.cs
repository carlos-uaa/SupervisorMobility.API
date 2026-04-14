using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities
{
    public class HRImages
    {
        [Key]
        public int ImageId { get; set; }
        public int? HriId { get; set; }
        public HRI?  HRI { get; set; }
        public string ImageUrl { get; set; }
        public string ImageType { get; set; }
    }
}
