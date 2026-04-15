using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;

namespace SupervisorMobility.API.Controllers.HRIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HRIHourmeterRevisionController : ControllerBase
    {
        private readonly IHRIHourmeterRevisionService _hourmeterRevisionService;
        public HRIHourmeterRevisionController(IHRIHourmeterRevisionService hourmeterRevisionService)
        {
            _hourmeterRevisionService = hourmeterRevisionService;
        }

        [HttpGet("GetAllHourmeterRevisions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<List<GetHourmeterRevisionDto>>>> GetAllHourmeterRevisions()
        {
            var response = await _hourmeterRevisionService.GetAllHourmeterRevisions();
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("GetHourmeterRevisionByHRIId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<GetHourmeterRevisionDto>>> GetHourmeterRevisionByHRIId(int HriId)
        {
            var response = await _hourmeterRevisionService.GetHourmeterRevisionByHRIId(HriId);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("GetHourmeterRevisionById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<GetHourmeterRevisionDto>>> GetHourmeterRevisionById(int id)
        {
            var response = await _hourmeterRevisionService.GetHourmeterRevisionById(id);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("AddHourmeterRevision")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<GetHourmeterRevisionDto>>> AddHourmeterRevision(CreateHourMeterRevisionDto newHourmeterRevision)
        {
            var response = await _hourmeterRevisionService.AddHourmeterRevision(newHourmeterRevision);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
    }

}
