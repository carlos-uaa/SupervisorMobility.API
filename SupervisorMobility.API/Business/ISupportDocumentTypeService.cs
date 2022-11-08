using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.SupportDocumentTypeDtos;

namespace SupervisorMobility.API.Business
{
    public interface ISupportDocumentTypeService
    {
        Task<IEnumerable<SupportDocumentType>> FetchSupportDocumentTypesAsync();
        Task<SupportDocumentType?> FetchSupportDocumentTypeAsync(int supportDocumentTypeId);
        Task<SupportDocumentType> CreateSupportDocumentTypeAsync(SupportDocumentType supportDocumentType);
        Task RemoveSupportDocumentTypeAsync(SupportDocumentType supportDocumentType);
        Task UpdateSupportDocumentTypeAsync(SupportDocumentTypeForUpdateDto supportDocumentTypeForUpdate, SupportDocumentType supportDocumentType);
    }
}
