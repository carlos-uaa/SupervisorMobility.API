using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIDtos.HRIMetrics;
using SupervisorMobility.API.Models.HRIWeeklyRevisions;

namespace SupervisorMobility.API.Controllers.HRIControllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class HRIController : ControllerBase
    {
        private readonly IHRIServices _HRIServices;
        public HRIController(IHRIServices HRIServices)
        {
            _HRIServices = HRIServices;
        }

        [HttpGet("GetAllHRI")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<List<GetHRIDto>>>> GetAllHRI()
        {
            var response = await _HRIServices.GetAllHRI();
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetHRIById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<GetHRIDto>>> GetHRIById(int id)
        {
            var response = await _HRIServices.GetHRIById(id);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetHRISoftInfoList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<List<GetHRIToTableDto>>>> GetHRISoftInfoList()
        {
            var response = await _HRIServices.GetAllHRITable();
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetDailyByMonthAndYear/{hriId}/{month}/{year}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<GetHRIDto>>> GetDailyByMonthAndYear(int hriId, int month, int year)
        {
            var response = await _HRIServices.GetDailyByMonthAndYear(hriId, month, year);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetHRIHistory/{hriId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<List<GetHRIHistoryActionDto>>>> GetHRIHistory(int hriId)
        {
            var response = await _HRIServices.GetHRIHistory(hriId);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetExcelHriFile/{hriId}/{month}/{year}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetExcelHriFile(int hriId, int month, int year)
        {
            var response = await _HRIServices.CreateExcelHriFile(hriId, month, year);
            if (response.Success == false)
            {
                return NotFound();
            }
            return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"HRI_{hriId}.xlsx");
        }

        [HttpPost("CreateHRI")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<GetHRIDto>>> CreateHRI(CreateHRIDto newHRI)
        {
            var response = await _HRIServices.CreateHRI(newHRI);
            if (response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("CreateNewWeeklyRevision")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<bool>>> CreateNewWeeklyRevision(List<CreateWeeklyRevisionDto> weeklyRevisions)
        {
            var response = await _HRIServices.CreateNewWeeeklyRevisions(weeklyRevisions);
            if (response.Data == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("UpdateHRI/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateHRI(int id, UpdateHRIDto updatedHRI)
        {
            var response = await _HRIServices.UpdateHRI(id, updatedHRI);
            if (!response.Data)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("DeleteHRI/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteHRI(int id)
        {
            var response = await _HRIServices.DeleteHRI(id);
            if (!response.Data)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        // Endpoints para el Dashboard del HRI
        [HttpGet("GetHriKPIs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<HriKpis>>> GetHriKPIs()
        {
            var response = await _HRIServices.GetHriKPIs();
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetLinesChartData/{distributionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<LinesChartData>>> GetLinesChartData(int areaId)
        {
            var response = await _HRIServices.GetLinesChartData(areaId);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetGeneralStatusChartData/{distributionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<GeneralStatusChartData>>> GetGeneralStatusChartData(int areaId)
        {
            var response = await _HRIServices.GetGeneralStatusChartData(areaId);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetRecentRevisions/{distributionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<List<HriRecentRevisionsDto>>>> GetRecentRevisions(int areaId, string? filter)
        {
            var response = await _HRIServices.GetRecentRevisions(areaId, filter);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}