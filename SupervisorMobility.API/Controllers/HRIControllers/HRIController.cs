using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRIDtos;
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
    }
}