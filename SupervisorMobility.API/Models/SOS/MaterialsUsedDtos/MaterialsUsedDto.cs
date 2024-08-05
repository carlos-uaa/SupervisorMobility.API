using SupervisorMobility.API.DataAccess.Entities.SOS;

namespace SupervisorMobility.API.Models.SOS.MaterialDtos
{
    public class MaterialsUsedDto
    {
        public int MaterialUsedId { get; set; }

        public int MaterialId { get; set; }
        public MaterialDto Material { get; set; }

        public double Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}
