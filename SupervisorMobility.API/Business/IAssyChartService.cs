using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.SupportDocumentTypeDtos;

namespace SupervisorMobility.API.Business
{
    public interface IAssyChartService
    {
        Task<IEnumerable<SupportDocumentType>> FetchSupportDocumentTypesAsync();
        Task<SupportDocumentType?> FetchSupportDocumentTypeAsync(int supportDocumentTypeId);
        Task<SupportDocumentType> CreateSupportDocumentTypeAsync(SupportDocumentType supportDocumentType);
        Task RemoveSupportDocumentTypeAsync(SupportDocumentType supportDocumentType);
        Task UpdateSupportDocumentTypeAsync(SupportDocumentTypeForUpdateDto supportDocumentTypeForUpdate, SupportDocumentType supportDocumentType);
        Task<bool> CheckPlantExistance(int plantId);
        Task<bool> CheckAreaExistance(int areaId);
        Task<bool> CheckDistributionExistance(int distributionId);
        Task<IEnumerable<Operation>> FetchOperationsAsync(int distributionId);
        Task<Operation?> FetchOperationAsync(int distributionId, int operationId);
        Task<Operation> CreateOperationAsync(int areaId, int distributionId, Operation operation);
        Task UpdateOperationAsync(OperationForUpdateDto operationForUpdate, Operation operation);
        Task RemoveOperationAsync(Operation operation);
    }
}
