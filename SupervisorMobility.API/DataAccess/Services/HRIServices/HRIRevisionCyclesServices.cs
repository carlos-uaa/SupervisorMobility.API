using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.Models.HRIRevisionCycles;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public class HRIRevisionCyclesServices : IHRIRevisionCyclesService
    {
        private readonly IHRIRevisionCyclesRepository _repository;
        public HRIRevisionCyclesServices(IHRIRevisionCyclesRepository repository)
        {
            _repository = repository;
        }   
        public async  Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCycles()
        {
                return await _repository.GetAllRevisionCycles();
        }

        public async Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCyclesByRevisionItemId(int itemId)
        {
            return await _repository.GetAllRevisionCyclesByRevisionItemId(itemId);
        }

        public async  Task<ServiceResponse<GetRevisionCyclesDto>> GetRevisionCycleById(int id)
        {
            return await _repository.GetRevisionCycleById(id);
        }

        public async Task<ServiceResponse<GetRevisionCyclesDto>> CreateRevisionCycle(int itemId, CreateRevisionCyclesDto createRevisionCyclesDto)
        {
            return await _repository.CreateRevisionCycle(itemId, createRevisionCyclesDto);
        }

        public async Task<ServiceResponse<bool>> CreateRevisionCyclesByRevisionItemId(int itemId, List<CreateRevisionCyclesDto> listOfRevisionsCycles)
        {
            return await _repository.CreateRevisionCyclesByRevisionItemId(itemId, listOfRevisionsCycles);
        }

        public async Task<ServiceResponse<GetRevisionCyclesDto>> UpdateRevisionCycle(int id, UpdateRevisionCycleDto updateRevisionCycleDto)
        {
            return await _repository.UpdateRevisionCycle(id, updateRevisionCycleDto);
        }

        public async Task<ServiceResponse<bool>> DeleteRevisionCycle(int id)
        {
            return await _repository.DeleteRevisionCycle(id);
        }
    }
}
