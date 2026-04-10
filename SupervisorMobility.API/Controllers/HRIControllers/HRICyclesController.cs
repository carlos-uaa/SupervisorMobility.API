using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRICyclesDtos;

namespace SupervisorMobility.API.Controllers.HRIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HRICyclesController : ControllerBase
    {
        private readonly IHRICyclesService _HRICyclesService;
        public HRICyclesController(IHRICyclesService HRICyclesService)
        {
            _HRICyclesService = HRICyclesService;
        }

        [HttpGet("GetAllHRICycles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<List<GetHRICyclesDto>>>> GetAllHRICycles()
        {
            var response = await _HRICyclesService.GetHRICycles();
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);

        }

            [HttpGet("GetHRICycleById/{id}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<ServiceResponse<GetHRICyclesDto>>> GetHRICycleById(int id)
            {
                var response = await _HRICyclesService.GetHRICycleById(id);
                if (response.Data == null)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
             [HttpPost("CreateHRICycle")]
             [ProducesResponseType(StatusCodes.Status201Created)]
             [ProducesResponseType(StatusCodes.Status400BadRequest)]
             [ProducesResponseType(StatusCodes.Status500InternalServerError)]
             public async Task<ActionResult<ServiceResponse<GetHRICyclesDto>>> CreateHRICycle(CreateHRICycles createHRICycle)
             {
                 var response = await _HRICyclesService.CreateHRICycle(createHRICycle);
                 if (response.Data == null)
                 {
                     return BadRequest(response);
                 }
                 return Ok(response);
             }
              [HttpDelete("DeleteHRICycle/{id}")]
              [ProducesResponseType(StatusCodes.Status200OK)]
              [ProducesResponseType(StatusCodes.Status404NotFound)]
              [ProducesResponseType(StatusCodes.Status500InternalServerError)]
              public async Task<ActionResult<ServiceResponse<bool>>> DeleteHRICycle(int id)
              {
                  var response = await _HRICyclesService.DeleteHRICycle(id);
                  if (response.Data == null || response.Data == false)
                  {
                      return NotFound(response);
                  }
                  return Ok(response);
        }
    }
}
