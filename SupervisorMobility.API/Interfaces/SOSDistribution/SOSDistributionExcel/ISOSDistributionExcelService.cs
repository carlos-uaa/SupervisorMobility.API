using Microsoft.AspNetCore.Mvc;

namespace SupervisorMobility.API.Interfaces.SOSDistribution.SOSDistributionExcel
{
    public interface ISOSDistributionExcelService
    {
        Task<string?> GetFileName(int sosDistributionId);
        Task<MemoryStream?> ExportSOSDistributionExcel(int sosDistributionId);
    }   
}
