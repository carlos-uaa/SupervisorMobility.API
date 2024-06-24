namespace SupervisorMobility.API.Models.SOS.MaterialDtos
{
    public class MaterialForUpdateDto
    {
        public int MaterialId { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public bool? IsActive { get; set; }
    }
}
