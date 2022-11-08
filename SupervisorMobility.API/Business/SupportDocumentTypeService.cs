using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.SupportDocumentTypeDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Business
{
    public class SupportDocumentTypeService : ISupportDocumentTypeService
    {
        private readonly ISupervisorMobilityRepository _repository;
        private readonly IMapper _mapper;

        public SupportDocumentTypeService(ISupervisorMobilityRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

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
    }
}
