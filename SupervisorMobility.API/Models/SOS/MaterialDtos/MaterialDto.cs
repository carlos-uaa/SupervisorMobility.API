namespace SupervisorMobility.API.Models.SOS.MaterialDtos
{
    public class MaterialDto
    {
        public int MaterialId { get; set; }
        public string? key { get; set; }
        public string? PartName { get; set; }
        public string? PartNumber { get; set; }
        public bool? IsActive { get; set; }
    }
}
