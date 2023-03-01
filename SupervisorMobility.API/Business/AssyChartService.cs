using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.ProductDtos;
using SupervisorMobility.API.Models.ProductOperationDtos;
using SupervisorMobility.API.Models.SupportDocumentTypeDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Business
{
    public class AssyChartService : IAssyChartService
    {
        private readonly ISupervisorMobilityRepository _repository;
        private readonly IMapper _mapper;

        public AssyChartService(ISupervisorMobilityRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        #region Product
        public async Task<bool> CheckProductExistance(int productId)
        {
            return await _repository.ProductExistAsync(productId);
        }
        #endregion
        #region Area

        public async Task<bool> CheckAreaExistance(int areaId)
        {
            return await _repository.AreaExistAsync(areaId);
        }
        #endregion
        #region Distribution
        public async Task<bool> CheckDistributionExistance(int distributionId)
        {
            return await _repository.DistributionExistsAsync(distributionId);
        }
        #endregion

        #region ProductDistribution
        public async Task<bool> CheckProductDistributionExistance(int productDistributionId)
        {
            return await _repository.ProductDistributionExistsAsync(productDistributionId);
        }
        #endregion

        #region Operation
        public async Task<Operation> CreateOperationAsync(int areaId, int distributionId, Operation operation)
        {
            await _repository.AddOperationForDistributionAsync(areaId, distributionId, operation);
            await _repository.SaveChangesAsync();
            return operation;

        }
        public async Task<Operation?> FetchOperationAsync(int distributionId, int operationId)
        {
            return await _repository.GetOperationForDistributionAsync(distributionId, operationId);
        }

        public async Task<IEnumerable<Operation>> FetchOperationsAsync(int distributionId)
        {
            return await _repository.GetOperationsForDistributionAsync(distributionId);
        }

        public async Task RemoveOperationAsync(Operation operation)
        {
            _repository.DeleteOperation(operation);
            await _repository.SaveChangesAsync();
        }
        public async Task UpdateOperationAsync(
            OperationForUpdateDto operationForUpdate,
            Operation operation)
        {
            _mapper.Map(operationForUpdate, operation);
            await _repository.SaveChangesAsync();
        }


        #endregion

        #region ProductOperation

        public async Task<ProductOperation> CreateProductOperationAsync(int productId, int productDistributionId, ProductOperation productOperation)
        {
            await _repository.AddProductOperationForDistributionAsync(productId, productDistributionId, productOperation);
            await _repository.SaveChangesAsync();
            return productOperation;

        }

        public async Task<IEnumerable<ProductOperation>> FetchProductOperationsAsync(int productDistributionId)
        {
            return await _repository.GetProductOperationsForDistributionAsync(productDistributionId);
        }

        public async Task<ProductOperation?> FetchProductOperationAsync(int productDistributionId, int productOperationId)
        {
            return await _repository.GetProductOperationForDistributionAsync(productDistributionId, productOperationId);
        }

        public async Task RemoveProductOperationAsync(ProductOperation productOperation)
        {
            _repository.DeleteProductOperation(productOperation);
            await _repository.SaveChangesAsync();
        }
        public async Task UpdateProductOperationAsync(
            ProductOperationForUpdateDto productOperationForUpdate,
            ProductOperation productOperation)
        {
            _mapper.Map(productOperationForUpdate, productOperation);
            await _repository.SaveChangesAsync();
        }

        #endregion
        #region Product
        public async Task<Product> CreateProductAsync(ProductForCreationDto product)
        {
            var productEntity = _mapper.Map<Product>(product);
            _repository.AddProduct(productEntity);
            await _repository.SaveChangesAsync();
            return productEntity;

        }

        public async Task<Product?> FetchProductAsync(int productId)
        {
            return await _repository.GetProductAsync(productId);
        }

        public async Task<IEnumerable<Product>> FetchProductsAsync()
        {
            return await _repository.GetProductsAsync();
        }


        public async Task RemoveProductAsync(Product product)
        {
            _repository.DeleteProduct(product);
            await _repository.SaveChangesAsync();
        }


        public async Task UpdateProductAsync(ProductForUpdateDto productForUpdate, Product product)
        {
            _mapper.Map(productForUpdate, product);
            await _repository.SaveChangesAsync();
        }


        #endregion
        #region Plant
        public async Task<IEnumerable<Plant>> FetchPlantsAsync()
        {
            return await _repository.GetPlantsAsync();
        }
        public async Task<Plant?> FetchPlantAsync(int plantId, bool includeAreas = false)
        {
            return await _repository.GetPlantAsync(plantId, includeAreas);
        }
        public async Task<Plant> CreatePlantAsync(PlantForCreationDto plant)
        {
            var finalPlant = _mapper.Map<Entities.Plant>(plant);
            _repository.AddPlant(finalPlant);
            await _repository.SaveChangesAsync();

            return finalPlant;
        }
        public async Task UpdatePlantAsync(PlantForUpdateDto plantForUpdate, Plant plant)
        {
            _mapper.Map(plantForUpdate, plant);
            await _repository.SaveChangesAsync();
        }
        public async Task RemovePlantAsync(Plant plant)
        {
            _repository.DeletePlant(plant);
            await _repository.SaveChangesAsync();
        }
        public async Task<bool> CheckPlantExistance(int plantId)
        {
            return await _repository.PlantExistAsync(plantId);
        }
        #endregion
        #region SupportDocumentTypes
        public async Task<SupportDocumentType> CreateSupportDocumentTypeAsync(SupportDocumentType supportDocumentType)
        {
            _repository.AddSupportDocumentType(supportDocumentType);
            await _repository.SaveChangesAsync();
            return supportDocumentType;
        }
        public async Task<SupportDocumentType?> FetchSupportDocumentTypeAsync(int supportDocumentTypeId)
        {
            return await _repository
                .GetSupportDocumentTypeAsync(supportDocumentTypeId);
        }
        public async Task<IEnumerable<SupportDocumentType>> FetchSupportDocumentTypesAsync()
        {
            return await _repository.GetSupportDocumentTypesAsync();
        }
        public async Task RemoveSupportDocumentTypeAsync(SupportDocumentType supportDocumentType)
        {
            _repository.DeleteSupportDocumentType(supportDocumentType);
            await _repository.SaveChangesAsync();
        }
        public async Task UpdateSupportDocumentTypeAsync(
            SupportDocumentTypeForUpdateDto supportDocumentTypeForUpdate,
            SupportDocumentType supportDocumentType)
        {
            _mapper.Map(supportDocumentTypeForUpdate, supportDocumentType);
            await _repository.SaveChangesAsync();
        }
        #endregion
        #region AssyChart
        public async Task<AssyChart> CreateAssyChartAsync(AssyChartForCreation assyChart)
        {
            var finalasssychart = _mapper.Map<AssyChart>(assyChart);
            _repository.AddAssyChartAsync(finalasssychart);
            await _repository.SaveChangesAsync();
            return finalasssychart;
        }

        public async Task UpdateAssyChartAsync(AssyChartForUpdateDto assyChartUpdate, AssyChart assyChart)
        {
            _mapper.Map(assyChartUpdate, assyChart);
            await _repository.SaveChangesAsync();
        }

        public async Task RemoveAssyChartAsync(AssyChart assyChart) 
        {
            _repository.DeleteAssyChartAsync(assyChart);
            await _repository.SaveChangesAsync();
        }

        #endregion

        #region User

        public async Task<User?> FetchUserAsync(int userId)
        {
            return await _repository.GetUserAsync(userId);
        }
        public async Task<User> CreateUserAsync(UsersForCreation newuser)
        {
            var finaluser = _mapper.Map<User>(newuser);
            _repository.AddUserAsync(finaluser);
            await _repository.SaveChangesAsync();
            return finaluser;
        }

        public async Task UpdateUserAsync(UsersForUpdateDto updateuser, User user)
        {
            _mapper.Map(updateuser, user);
            await _repository.SaveChangesAsync();
        }

        public async Task RemoveUserAsync(User user)
        {
            _repository.DeleteUserAsync(user);
            await _repository.SaveChangesAsync();
        }

        #endregion


    }
}
