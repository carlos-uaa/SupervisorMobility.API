namespace SupervisorMobility.API.Models.SOS.EquipmentDtos
{
    public class EquipmentDto
    {
        public int EquipmentId { get; set; }
        public string EquipmentCode { get; set; }
        public string EquipmentName { get; set; }
        public bool? IsActive { get; set; }
    }
}
