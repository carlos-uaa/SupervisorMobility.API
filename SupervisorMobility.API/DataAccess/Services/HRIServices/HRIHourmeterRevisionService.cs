using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public class HRIHourmeterRevisionService : IHRIHourmeterRevisionService
    {
        public Task<ServiceResponse<List<GetHourmeterRevisionDto>>> GetAllHourmeterRevisions()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetHourmeterRevisionDto>> GetHourmeterRevisionByHRIId(int Hrid)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetHourmeterRevisionDto>> GetHourmeterRevisionById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetHourmeterRevisionDto>> AddHourmeterRevision(CreateHourMeterRevisionDto newHourmeterRevision)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<GetHourmeterRevisionDto>>> DeleteHourmeterRevision(int id)
        {
            throw new NotImplementedException();
        }
    }
}
