using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIWeeklyRevisions;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public interface IHRIServices
    {
        Task<ServiceResponse<List<GetHRIDto>>>  GetAllHRI();
        Task<ServiceResponse<GetHRIDto>> GetHRIById(int id);
        Task<ServiceResponse<GetHRIDto>> CreateHRI(CreateHRIDto newHRI);
        Task<ServiceResponse<bool>> CreateNewWeeeklyRevisions(List<CreateWeeklyRevisionDto> weeklyRevisions);
        Task<ServiceResponse<List<GetHRIToTableDto>>> GetAllHRITable();
        Task<ServiceResponse<bool>>DeleteHRI(int id);
        
    }
}
