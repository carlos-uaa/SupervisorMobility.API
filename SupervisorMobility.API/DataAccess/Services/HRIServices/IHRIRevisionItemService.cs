using SupervisorMobility.API.Models.HRIRevisionItemsDtos;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public interface IHRIRevisionItemService
    {
        Task<ServiceResponse<List<GetHRIRevisionItemDto>>> GetAllHRIRevisionItems();
        Task<ServiceResponse<GetHRIRevisionItemDto>> GetHRIRevisionItemById(int id);
        Task<ServiceResponse<GetHRIRevisionItemDto>> CreateHRIRevisionItem(CreateHRIRevisionItemDto createHRIRevisionItemDto);
        Task<ServiceResponse<bool>> CreateHRIREvisionItemsByHRIId(int hriId, List<CreateHRIRevisionItemDto> createHRIRevisionItemDtos, int numOfCycles  );
        Task<ServiceResponse<GetHRIRevisionItemDto>> UpdateHRIRevisionItem(int id, UpdateHRIRevisionItemDto updateHRIRevisionItemDto);
        Task<ServiceResponse<bool>>  DeleteHRIRevisionItem(int id);

        Task<ServiceResponse<List<GetFrequencyDto>>> GetAllFrequencies();
        Task<ServiceResponse<GetFrequencyDto>> GetFrequencyById(int id);
        Task<ServiceResponse<GetFrequencyDto>> CreateFrequency(CreateFrequencyDto createFrequencyDto);
        Task<ServiceResponse<GetFrequencyDto>> UpdateFrequency(int id, UpdateFrequencyDto updateFrequencyDto);
        Task<ServiceResponse<bool>> DeleteFrequency(int id);

        Task<ServiceResponse<List<GetVeredictDto>>> GetAllVeredicts();  
        Task<ServiceResponse<GetVeredictDto>> GetVeredictById(int id);
        Task<ServiceResponse<GetVeredictDto>> CreateVeredict(CreateVeredictDto createVeredictDto);
        Task<ServiceResponse<GetVeredictDto>> UpdateVeredict(int id, UpdateVeredictDto updateVeredictDto);
        Task<ServiceResponse<bool>> DeleteVeredict(int id);

        Task<ServiceResponse<List<GetRevisionMethodDto>>> GetAllRevisionMethods();
        Task<ServiceResponse<GetRevisionMethodDto>> GetRevisionMethodById(int id);
        Task<ServiceResponse<GetRevisionMethodDto>> CreateRevisionMethod(CreateRevisionMethodDto createRevisionMethodDto);
        Task<ServiceResponse<GetRevisionMethodDto>> UpdateRevisionMethod(int id, UpdateRevisionMethodDto updateRevisionMethodDto);
        Task<ServiceResponse<bool>> DeleteRevisionMethod(int id);
    }
}
