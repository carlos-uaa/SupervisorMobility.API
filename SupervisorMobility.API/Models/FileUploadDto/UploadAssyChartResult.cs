using System.Reflection;

namespace SupervisorMobility.API.Models.FileUpload
{
     public class UploadAssyChartResult
    {
        public int PlantUpdate { get; set; } = 0;
        public int PlantCreate { get; set; }= 0;

        public int AreasCreated { get; set; } = 0;
        public int AreasUpdated { get; set; } = 0;

        public int DistributionCreated { get; set; } = 0;
        public int DistributionUpdated { get; set; } = 0;

        public int OperationCreated { get; set; } = 0;
        public int OperationUpdated { get; set; } = 0;

        public int ProductCreated { get; set; } = 0;
        public int ProductUpdated { get; set; } = 0;

        public int AssyChartCreated { get; set; } = 0;
        public int AssyChartUpdated { get; set; } = 0;
    }
}
