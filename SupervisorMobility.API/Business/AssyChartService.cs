using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.SupportDocumentTypeDtos;
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

        public async Task<bool> CheckAreaExistance(int areaId)
        {
            return await _repository.AreaExistAsync(areaId);
        }

        public async Task<bool> CheckDistributionExistance(int distributionId)
        {
            return await _repository.DistributionExistsAsync(distributionId);
        }

        public async Task<bool> CheckPlantExistance(int plantId)
        {
            return await _repository.PlantExistAsync(plantId);
        }

        public async Task<Operation> CreateOperationAsync(int areaId, int distributionId, Operation operation)
        {
            await _repository.AddOperationForDistributionAsync(areaId, distributionId, operation);
            await _repository.SaveChangesAsync();
            return (operation);

        }

        public async Task<SupportDocumentType> CreateSupportDocumentTypeAsync(SupportDocumentType supportDocumentType)
        {
            _repository.AddSupportDocumentType(supportDocumentType);
            await _repository.SaveChangesAsync();
            return supportDocumentType;
        }

        public async Task<Operation?> FetchOperationAsync(int distributionId, int operationId)
        {
            return await _repository.GetOperationForDistributionAsync(distributionId, operationId);
        }

        public async Task<IEnumerable<Operation>> FetchOperationsAsync(int distributionId)
        {
            return await _repository.GetOperationsForDistributionAsync(distributionId);
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

        public async Task RemoveOperationAsync(Operation operation)
        {
            _repository.DeleteOperation(operation);
            await _repository.SaveChangesAsync();
        }

        public async Task RemoveSupportDocumentTypeAsync(SupportDocumentType supportDocumentType)
        {
            _repository.DeleteSupportDocumentType(supportDocumentType);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateOperationAsync(
            OperationForUpdateDto operationForUpdate, 
            Operation operation)
        {
            _mapper.Map(operationForUpdate, operation);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateSupportDocumentTypeAsync(
            SupportDocumentTypeForUpdateDto supportDocumentTypeForUpdate,
            SupportDocumentType supportDocumentType)
        {
            _mapper.Map(supportDocumentTypeForUpdate, supportDocumentType);
            await _repository.SaveChangesAsync();
        }
    }
}
