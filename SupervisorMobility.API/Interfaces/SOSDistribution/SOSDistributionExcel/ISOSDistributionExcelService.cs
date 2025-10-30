using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.SOS;

namespace SupervisorMobility.API.Interfaces.SOSDistribution.SOSDistributionExcel
{
    public interface ISOSDistributionExcelService
    {
        Task<string?> GetFileName(int sosDistributionId);
        Task<MemoryStream?> ExportSOSDistributionExcel(int sosDistributionId);
       
    }   
}
