using SupervisorMobility.API.Models.HRICyclesDtos;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public interface IHRICyclesService
    {
            Task<ServiceResponse<List<GetHRICyclesDto>>> GetHRICycles();
            Task<ServiceResponse<GetHRICyclesDto>> GetHRICycleById(int id);
            Task<ServiceResponse<GetHRICyclesDto>> CreateHRICycle(CreateHRICyclesDto createHRICycle);
            Task<ServiceResponse<GetHRICyclesDto>> UpdateHRICycle(int id, UpdateHRICycleDto updateHRICycle);
            Task<ServiceResponse<bool>> DeleteHRICycle(int id);
    }

}
