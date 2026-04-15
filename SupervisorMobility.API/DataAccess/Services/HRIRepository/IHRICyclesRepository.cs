using SupervisorMobility.API.Models.HRICyclesDtos;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public interface IHRICyclesRepository
    {
        Task<ServiceResponse<List<GetHRICyclesDto>>> GetHRICycles();
        Task<ServiceResponse<GetHRICyclesDto>> GetHRICycleById(int id);
        Task<ServiceResponse<GetHRICyclesDto>> CreateHRICycle(CreateHRICyclesDto createHRICycle);
        Task<ServiceResponse<bool>> CreateHRICyclesByHRIId(int hriId, List<CreateHRICyclesDto> createHRICycles);
        Task<ServiceResponse<bool>> CreateNewDailyRevision(CreateDailyRevisionDto createDaily);
        Task<ServiceResponse<GetHRICyclesDto>> UpdateHRICycle(int id, UpdateHRICycleDto updateHRICycle);
        Task<ServiceResponse<bool>> DeleteHRICycle(int id);
    }
}
