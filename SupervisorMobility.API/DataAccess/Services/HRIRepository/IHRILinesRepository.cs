using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public interface IHRILinesRepository
    {
        Task<ServiceResponse<List<HRILines>>> GetAllHRILinesAsync();
        Task<ServiceResponse<HRILines>> GetSingleHRILineAsync(int Id);
        Task<ServiceResponse<HRILines>> CreateHRILineAsync(HRILines Line);
        Task<ServiceResponse<HRILines>> UpdateHRILineAsync(HRILines Line);
        Task<ServiceResponse<bool>> DeleteHRILineAsync(int Id);
    }
}
