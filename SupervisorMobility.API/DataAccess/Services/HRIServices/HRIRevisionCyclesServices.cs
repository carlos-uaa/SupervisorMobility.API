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
        public Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCycles()
        {
                return _repository.GetAllRevisionCycles();
        }

        public Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCyclesByRevisionItemId(int itemId)
        {
            return _repository.GetAllRevisionCyclesByRevisionItemId(itemId);
        }

        public Task<ServiceResponse<GetRevisionCyclesDto>> GetRevisionCycleById(int id)
        {
            return _repository.GetRevisionCycleById(id);
        }

        public Task<ServiceResponse<GetRevisionCyclesDto>> CreateRevisionCycle(int itemId, CreateRevisionCyclesDto createRevisionCyclesDto)
        {
            return _repository.CreateRevisionCycle(itemId, createRevisionCyclesDto);
        }

        public Task<ServiceResponse<bool>> CreateRevisionCyclesByRevisionItemId(int itemId, List<CreateRevisionCyclesDto> listOfRevisionsCycles)
        {
            return _repository.CreateRevisionCyclesByRevisionItemId(itemId, listOfRevisionsCycles);
        }

        public Task<ServiceResponse<GetRevisionCyclesDto>> UpdateRevisionCycle(int id, UpdateRevisionCycleDto updateRevisionCycleDto)
        {
            return _repository.UpdateRevisionCycle(id, updateRevisionCycleDto);
        }

        public Task<ServiceResponse<bool>> DeleteRevisionCycle(int id)
        {
            return _repository.DeleteRevisionCycle(id);
        }
    }
}
