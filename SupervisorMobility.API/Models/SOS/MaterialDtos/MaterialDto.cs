namespace SupervisorMobility.API.Models.SOS.MaterialDtos
{
    public class MaterialDto
    {
        public int MaterialId { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public bool? IsActive { get; set; }
    }
}
