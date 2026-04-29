using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIWeeklyRevisions;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public interface IHRIRepository
    {
        Task<ServiceResponse<List<GetHRIDto>>> GetAllHRI();
        Task<ServiceResponse<GetHRIDto>> GetHRIById(int id);
        Task<ServiceResponse<GetHRIDto>> CreateHRI(CreateHRIDto newHRI);
        Task<ServiceResponse<bool>> UpdateHRI(int id, UpdateHRIDto updatedHRI);
        Task<ServiceResponse<bool>> CreateNewWeeeklyRevisions(List<CreateWeeklyRevisionDto> weeklyRevisions);
        Task<ServiceResponse<bool>> DeleteHRI(int id);
        Task<ServiceResponse<List<GetHRIToTableDto>>> GetAllHRITable();
        Task<ServiceResponse<List<GetHRIHistoryActionDto>>> GetHRIHistory(int hriId);
    }
}
