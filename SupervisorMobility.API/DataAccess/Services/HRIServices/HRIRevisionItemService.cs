using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;


namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public class HRIRevisionItemService : IHRIRevisionItemService
    {
        private readonly IHRIRevisionItemRepository _hriRevisionItemRepository;
        public HRIRevisionItemService(IHRIRevisionItemRepository hriRevisionItemRepository)
        {
            _hriRevisionItemRepository = hriRevisionItemRepository;
        }
        #region revision item
        public async  Task<ServiceResponse<List<GetHRIRevisionItemDto>>> GetAllHRIRevisionItems()
        {
            return await _hriRevisionItemRepository.GetAllHRIRevisionItems();
        }

        public async Task<ServiceResponse<GetHRIRevisionItemDto>> GetHRIRevisionItemById(int id)
        {
            return await _hriRevisionItemRepository.GetHRIRevisionItemById(id);   
        }

        public async Task<ServiceResponse<GetHRIRevisionItemDto>> CreateHRIRevisionItem(CreateHRIRevisionItemDto createHRIRevisionItemDto)
        {
            return await _hriRevisionItemRepository.CreateHRIRevisionItem(createHRIRevisionItemDto);
        }

        public async Task<ServiceResponse<GetHRIRevisionItemDto>> UpdateHRIRevisionItem(int id, UpdateHRIRevisionItemDto updateHRIRevisionItemDto)
        {
            return await _hriRevisionItemRepository.UpdateHRIRevisionItem(id, updateHRIRevisionItemDto);
        }

        public async Task<ServiceResponse<bool>> DeleteHRIRevisionItem(int id)
        {
            return await _hriRevisionItemRepository.DeleteHRIRevisionItem(id);
        }
        public async Task<ServiceResponse<bool>> CreateHRIREvisionItemsByHRIId(int hriId, List<CreateHRIRevisionItemDto> createHRIRevisionItemDtos, int numOfCycles)
        {
            return await _hriRevisionItemRepository.CreateHRIREvisionItemsByHRIId(hriId, createHRIRevisionItemDtos, numOfCycles);
        }
        #endregion
        #region frequency
        public async Task<ServiceResponse<List<GetFrequencyDto>>> GetAllFrequencies()
        {
            return await _hriRevisionItemRepository.GetAllFrequencies();
        }

        public async Task<ServiceResponse<GetFrequencyDto>> GetFrequencyById(int id)
        {
            return await _hriRevisionItemRepository.GetFrequencyById(id);
        }

        public async Task<ServiceResponse<GetFrequencyDto>> CreateFrequency(CreateFrequencyDto createFrequencyDto)
        {
            return await _hriRevisionItemRepository.CreateFrequency(createFrequencyDto);
        }

        public async Task<ServiceResponse<GetFrequencyDto>> UpdateFrequency(int id, UpdateFrequencyDto updateFrequencyDto)
        {
            return await _hriRevisionItemRepository.UpdateFrequency(id, updateFrequencyDto);
        }

        public async Task<ServiceResponse<bool>> DeleteFrequency(int id)
        {
            return await _hriRevisionItemRepository.DeleteFrequency(id);
        }
        #endregion

        #region veredict
        public async Task<ServiceResponse<List<GetVeredictDto>>> GetAllVeredicts()
        {
            return await _hriRevisionItemRepository.GetAllVeredicts();
        }

        public async Task<ServiceResponse<GetVeredictDto>> GetVeredictById(int id)
        {
            return await _hriRevisionItemRepository.GetVeredictById(id);
        }

        public async Task<ServiceResponse<GetVeredictDto>> CreateVeredict(CreateVeredictDto createVeredictDto)
        {
            return await _hriRevisionItemRepository.CreateVeredict(createVeredictDto);
        }

        public async Task<ServiceResponse<GetVeredictDto>> UpdateVeredict(int id, UpdateVeredictDto updateVeredictDto)
        {
            return await _hriRevisionItemRepository.UpdateVeredict(id, updateVeredictDto);
        }

        public async Task<ServiceResponse<bool>> DeleteVeredict(int id)
        {
            return await _hriRevisionItemRepository.DeleteVeredict(id);
        }
        #endregion
        #region revision method
        public async Task<ServiceResponse<List<GetRevisionMethodDto>>> GetAllRevisionMethods()
        {
            return await _hriRevisionItemRepository.GetAllRevisionMethods();
        }

        public async Task<ServiceResponse<GetRevisionMethodDto>> GetRevisionMethodById(int id)
        {
            return await _hriRevisionItemRepository.GetRevisionMethodById(id);
        }

        public async Task<ServiceResponse<GetRevisionMethodDto>> CreateRevisionMethod(CreateRevisionMethodDto createRevisionMethodDto)
        {
            return await _hriRevisionItemRepository.CreateRevisionMethod(createRevisionMethodDto);
        }

        public async Task<ServiceResponse<GetRevisionMethodDto>> UpdateRevisionMethod(int id, UpdateRevisionMethodDto updateRevisionMethodDto)
        {
            return await _hriRevisionItemRepository.UpdateRevisionMethod(id, updateRevisionMethodDto);
        }

        public async Task<ServiceResponse<bool>> DeleteRevisionMethod(int id)
        {
            return await _hriRevisionItemRepository.DeleteRevisionMethod(id );
        }
        #endregion
    }
}
