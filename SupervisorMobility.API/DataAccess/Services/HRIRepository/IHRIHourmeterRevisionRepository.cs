using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;

namespace SupervisorMobility.API.DataAccess.Services.HRIRepository
{
    public interface IHRIHourmeterRevisionRepository
    {
        Task<ServiceResponse<List<GetHourmeterRevisionDto>>> GetAllHourmeterRevisions();
        Task<ServiceResponse<GetHourmeterRevisionDto>> GetHourmeterRevisionByHRIId(int Hrid);
        Task<ServiceResponse<GetHourmeterRevisionDto>> GetHourmeterRevisionById(int id);
        Task<ServiceResponse<GetHourmeterRevisionDto>> AddHourmeterRevision(CreateHourMeterRevisionDto newHourmeterRevision);
        Task<ServiceResponse<List<GetHourmeterRevisionDto>>> DeleteHourmeterRevision(int id);
    }
}
