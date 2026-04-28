using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public interface IHRIDocksRepository
    {
        Task<ServiceResponse<List<HRIDock>>> GetAllHRIDocksAsync();
        Task<ServiceResponse<HRIDock>> GetSingleHRIDockAsync(int Id);
        Task<ServiceResponse<HRIDock>> CreateHRIDockAsync(HRIDock Dock);
        Task<ServiceResponse<HRIDock>> UpdateHRIDockAsync(HRIDock Dock);
        Task<ServiceResponse<bool>> DeleteHRIDockAsync(int Id);
    }
}
