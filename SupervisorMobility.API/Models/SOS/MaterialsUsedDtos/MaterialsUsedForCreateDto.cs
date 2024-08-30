using SupervisorMobility.API.DataAccess.Entities.SOS;

namespace SupervisorMobility.API.Models.SOS.MaterialDtos
{
    public class MaterialsUsedForCreateDto
    {
        public int MaterialId { get; set; }

        public double Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}
