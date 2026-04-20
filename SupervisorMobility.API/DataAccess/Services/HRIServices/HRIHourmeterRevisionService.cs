using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public class HRIHourmeterRevisionService : IHRIHourmeterRevisionService
    {
        private readonly IHRIHourmeterRevisionRepository _hourmeterRevisionRepository;
        public HRIHourmeterRevisionService(IHRIHourmeterRevisionRepository hourmeterRevisionRepository)
        {
            _hourmeterRevisionRepository = hourmeterRevisionRepository;
        }   
        public async  Task<ServiceResponse<List<GetHourmeterRevisionDto>>> GetAllHourmeterRevisions()
        {
           return await _hourmeterRevisionRepository.GetAllHourmeterRevisions();
        }

        public async Task<ServiceResponse<GetHourmeterRevisionDto>> GetHourmeterRevisionByHRIId(int Hrid)
        {
            return await _hourmeterRevisionRepository.GetHourmeterRevisionByHRIId(Hrid);
        }

        public async  Task<ServiceResponse<GetHourmeterRevisionDto>> GetHourmeterRevisionById(int id)
        {
            return await _hourmeterRevisionRepository.GetHourmeterRevisionById(id);
        }

        public async Task<ServiceResponse<GetHourmeterRevisionDto>> AddHourmeterRevision(CreateHourMeterRevisionDto newHourmeterRevision)
        {
            return await _hourmeterRevisionRepository.AddHourmeterRevision(newHourmeterRevision);
        }

        public async Task<ServiceResponse<bool>> DeleteHourmeterRevision(int id)
        {
            return await _hourmeterRevisionRepository.DeleteHourmeterRevision(id);
        }
        public async Task<ServiceResponse<bool>> CreateNewDailyRevision(CreateDailyRevisionDto createDaily)
        {
            return await _hourmeterRevisionRepository.CreateNewDailyRevision(createDaily);
        }
    }
}
