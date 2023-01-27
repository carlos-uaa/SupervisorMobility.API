using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.ProductDtos;
using SupervisorMobility.API.Models.SupportDocumentTypeDtos;

namespace SupervisorMobility.API.Business
{
    public interface IAssyChartService
    {
        #region SupportDocumentTypes
        Task<IEnumerable<SupportDocumentType>> FetchSupportDocumentTypesAsync();
        Task<SupportDocumentType?> FetchSupportDocumentTypeAsync(int supportDocumentTypeId);
        Task<SupportDocumentType> CreateSupportDocumentTypeAsync(SupportDocumentType supportDocumentType);
        Task RemoveSupportDocumentTypeAsync(SupportDocumentType supportDocumentType);
        Task UpdateSupportDocumentTypeAsync(SupportDocumentTypeForUpdateDto supportDocumentTypeForUpdate, SupportDocumentType supportDocumentType);
        #endregion
        #region Operations
        Task<IEnumerable<Operation>> FetchOperationsAsync(int distributionId);
        Task<Operation?> FetchOperationAsync(int distributionId, int operationId);
        Task<Operation> CreateOperationAsync(int areaId, int distributionId, Operation operation);
        Task UpdateOperationAsync(OperationForUpdateDto operationForUpdate, Operation operation);
        Task RemoveOperationAsync(Operation operation);
        #endregion
        #region Product
        Task<IEnumerable<Product>> FetchProductsAsync();
        Task<Product?> FetchProductAsync(int productId);
        Task<Product> CreateProductAsync(ProductForCreationDto product);
        Task UpdateProductAsync(ProductForUpdateDto productForUpdate, Product product);
        Task RemoveProductAsync(Product product);
        #endregion
        #region Plant
        Task<IEnumerable<Plant>> FetchPlantsAsync();
        Task<Plant?> FetchPlantAsync(int plantId, bool includeAreas = false);
        Task<Plant> CreatePlantAsync(PlantForCreationDto plant);
        Task UpdatePlantAsync(PlantForUpdateDto plantForUpdate, Plant plant);
        Task RemovePlantAsync(Plant plant);
        Task<bool> CheckPlantExistance(int plantId);
        #endregion
        #region Area
        Task<bool> CheckAreaExistance(int areaId);
        #endregion
        #region Distribution
        Task<bool> CheckDistributionExistance(int distributionId);
        #endregion

        #region AssyChart
        Task<AssyChart> CreateAssyChartAsync(AssyChart assyChart);
        Task UpdateAssyChartAsync(AssyChartForUpdateDto assyChartForUpdate, AssyChart assyChart);
        Task RemoveAssyChartAsync(AssyChart assyChart);
        #endregion




    }
}
