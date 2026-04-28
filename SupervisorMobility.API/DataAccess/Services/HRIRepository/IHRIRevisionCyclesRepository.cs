using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIRevisionCycles;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public interface IHRIRevisionCyclesRepository
    {
        Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCycles();
        Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCyclesByRevisionItemId(int itemId);
        Task<ServiceResponse<GetRevisionCyclesDto>> GetRevisionCycleById(int id);
        Task<ServiceResponse<GetRevisionCyclesDto>> CreateRevisionCycle(int itemId, CreateRevisionCyclesDto createRevisionCyclesDto);
        Task<ServiceResponse<bool>> CreateRevisionCyclesByRevisionItemId(int itemId, List<CreateRevisionCyclesDto> listOfRevisionsCycles);
        Task<ServiceResponse<bool>> CreateNewDailyRevision(CreateDailyRevisionDto createDaily);
        Task<ServiceResponse<GetRevisionCyclesDto>> UpdateRevisionCycle(int id, UpdateRevisionCycleDto updateRevisionCycleDto);
        Task<ServiceResponse<bool>> DeleteRevisionCycle(int id);
        Task<ServiceResponse<bool>> DeleteRevisionCycleByHriId(int hriId, int cycle);
        Task<ServiceResponse<bool>> AddNewRevisionCycleToRevisionsItems(int hriId, CreateRevisionCyclesDto newRevisionCycle);
    }
}
