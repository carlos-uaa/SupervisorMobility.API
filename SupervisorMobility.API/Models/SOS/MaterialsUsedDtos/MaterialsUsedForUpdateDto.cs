using SupervisorMobility.API.DataAccess.Entities.SOS;

namespace SupervisorMobility.API.Models.SOS.MaterialDtos
{
    public class MaterialsUsedForUpdateDto
    {
        public int MaterialUsedId { get; set; }

        public int MaterialId { get; set; }

        public double Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}
