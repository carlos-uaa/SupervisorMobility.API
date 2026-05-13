
using SupervisorMobility.API.DataAccess.Services.HRIRepository;
using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIDtos.HRIMetrics;
using SupervisorMobility.API.Models.HRIWeeklyRevisions;

namespace SupervisorMobility.API.DataAccess.Services.HRIServices
{
    public class HRIServices : IHRIServices
    {
        private readonly IHRIRepository _hriRepository;
        public HRIServices(IHRIRepository hriRepository)
        {
            _hriRepository = hriRepository;
        }

        public async Task<ServiceResponse<List<GetHRIDto>>> GetAllHRI()
        {
            return await _hriRepository.GetAllHRI();
        }

        public async Task<ServiceResponse<GetHRIDto>> GetHRIById(int id)
        {
            return await _hriRepository.GetHRIById(id);
        }

        public async Task<ServiceResponse<GetHRIDto>> CreateHRI(CreateHRIDto newHRI)
        {
            return await _hriRepository.CreateHRI(newHRI);
        }

        public async Task<ServiceResponse<bool>> DeleteHRI(int id)
        {
            return await _hriRepository.DeleteHRI(id);
        }
        public async Task<ServiceResponse<bool>> CreateNewWeeeklyRevisions(List<CreateWeeklyRevisionDto> weeklyRevisions)
        {
            return await _hriRepository.CreateNewWeeeklyRevisions(weeklyRevisions);
        }

        public async Task<ServiceResponse<List<GetHRIToTableDto>>> GetAllHRITable()
        {
            return await _hriRepository.GetAllHRITable();
        }

        public Task<ServiceResponse<bool>> UpdateHRI(int id, UpdateHRIDto updatedHRI)
        {
                        return _hriRepository.UpdateHRI(id, updatedHRI);    
        }
        public async Task<ServiceResponse<List<GetHRIHistoryActionDto>>> GetHRIHistory(int hriId)
        {
            return await _hriRepository.GetHRIHistory(hriId); 
        }

        // Endpoints para el Dashboard del HRI
        public async Task<ServiceResponse<HriKpis>> GetHriKPIs()
        {
            return await _hriRepository.GetHriKPIs();
        }

        public async Task<ServiceResponse<LinesChartData>> GetLinesChartData(int areaId)
        {
            return await _hriRepository.GetLinesChartData(areaId);
        }

        public async Task<ServiceResponse<GeneralStatusChartData>> GetGeneralStatusChartData(int areaId)
        {
            return await _hriRepository.GetGeneralStatusChartData(areaId);
        }

        public async Task<ServiceResponse<List<HriRecentRevisionsDto>>> GetRecentRevisions(int areaId, string? filter)
        {
            return await _hriRepository.GetRecentRevisions(areaId, filter);
        }
    }
}
