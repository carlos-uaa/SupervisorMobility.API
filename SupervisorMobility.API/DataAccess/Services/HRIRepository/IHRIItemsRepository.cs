using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public interface IHRIItemsRepository
    {
        Task<ServiceResponse<List<HRIItem>>> GetAllHRIItemsAsync();
        Task<ServiceResponse<HRIItem>> GetSingleHRIItemAsync(int Id);
        Task<ServiceResponse<HRIItem>> CreateHRIItemAsync(HRIItem Item);
        Task<ServiceResponse<HRIItem>> UpdateHRIItemAsync(HRIItem Item);
        Task<ServiceResponse<bool>> DeleteHRIItemAsync(int Id);
    }
}
