using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public interface IHRIHourmeterRevisionService
    {
        Task<ServiceResponse<List<GetHourmeterRevisionDto>>> GetAllHourmeterRevisions();
        Task<ServiceResponse<GetHourmeterRevisionDto>>  GetHourmeterRevisionByHRIId(int Hrid);
        Task<ServiceResponse<GetHourmeterRevisionDto>>  GetHourmeterRevisionById(int id);
        Task<ServiceResponse<GetHourmeterRevisionDto>> AddHourmeterRevision(CreateHourMeterRevisionDto newHourmeterRevision);        
        Task<ServiceResponse<List<GetHourmeterRevisionDto>>> DeleteHourmeterRevision(int id);
    }
}
