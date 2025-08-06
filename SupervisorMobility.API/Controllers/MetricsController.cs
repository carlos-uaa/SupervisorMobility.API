using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Models.HCIDtos;
using SupervisorMobility.API.Models.MetricsDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        public MetricsController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("totaljobs")]
        public async Task<ActionResult<int>> GetTotalJobObs([FromQuery]MetricsFiltersDto filters)
        {
            try
            {
                int total = await _supervisorMobilityRepository.GetTotalJobs(filters);
                return Ok(total);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, "Unexpected error ocurred");
            }
        }

        [HttpGet("jobsstatusdata")]
        public async Task<ActionResult<Dictionary<string,int>>> GetStatusDataForJobs([FromQuery] MetricsFiltersDto filters)
        {
            try
            {
                Dictionary<string, int> total = await _supervisorMobilityRepository.GetJobsStatusChartData(filters);
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Unexpected error ocurred");
            }
        }

        [HttpGet("jobstypedata")]
        public async Task<ActionResult<Dictionary<string, int>>> GetTypesDataForJobs([FromQuery] MetricsFiltersDto filters)
        {
            try
            {
                Dictionary<string, int> total = await _supervisorMobilityRepository.GetJobsTypeChartData(filters);
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Unexpected error ocurred");
            }
        }

        //LUP 

        [HttpGet("totallups")]
        public async Task<ActionResult<int>> GetTotalLUPs([FromQuery] MetricsFiltersDto filters)
        {
            try
            {
                int total = await _supervisorMobilityRepository.GetTotalLUPs(filters);
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Unexpected error ocurred");
            }
        }
        [HttpGet("lupdata")]
        public async Task<ActionResult<Dictionary<string, int>>> GetLUPData([FromQuery] MetricsFiltersDto filters)
        {
            try
            {
                Dictionary<string, int> total = await _supervisorMobilityRepository.GetLUPData(filters);
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Unexpected error ocurred");
            }
        }

        [HttpGet("lupProgressData")]
        public async Task<ActionResult<Dictionary<string, int>>> GetLUPProgressData([FromQuery] MetricsFiltersDto filters)
        {
            try
            {
                Dictionary<string, int> total = await _supervisorMobilityRepository.GetLUPProgressData(filters);
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Unexpected error ocurred");
            }
        }
    }
}
