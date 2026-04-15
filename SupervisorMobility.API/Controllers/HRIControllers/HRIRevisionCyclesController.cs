using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIRevisionCycles;

namespace SupervisorMobility.API.Controllers.HRIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HRIRevisionCyclesController : ControllerBase
    {
        private readonly IHRIRevisionCyclesService _service;
        public HRIRevisionCyclesController(IHRIRevisionCyclesService service)
        {
            _service = service;
        }

        [HttpGet("GetAllRevisionCycles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCycles()
        {
            return await _service.GetAllRevisionCycles();
        }

        [HttpGet("GetAllRevisionCyclesByRevisionItemId/{itemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCyclesByRevisionItemId(int itemId)
        {
            return await _service.GetAllRevisionCyclesByRevisionItemId(itemId);
        }

        [HttpGet("GetRevisionCycleById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ServiceResponse<GetRevisionCyclesDto>> GetRevisionCycleById(int id)
        {
            return await _service.GetRevisionCycleById(id);
        }

        [HttpPost("CreateRevisionCycle/{itemId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ServiceResponse<GetRevisionCyclesDto>> CreateRevisionCycle(int itemId, CreateRevisionCyclesDto dto)
        {
            return await _service.CreateRevisionCycle(itemId, dto);
        }

        [HttpPost("CreateRevisionCyclesByRevisionItemId/{itemId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ServiceResponse<bool>> CreateRevisionCyclesByRevisionItemId(int itemId, List<CreateRevisionCyclesDto> listOfRevisionsCycles)
        {
            return await _service.CreateRevisionCyclesByRevisionItemId(itemId, listOfRevisionsCycles);
        }

        [HttpPost("CreateNewDailyRevisionsForRevisionCycle")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ServiceResponse<bool>> CreateNewDailyRevision(CreateDailyRevisionDto createDaily)
        {
            return await _service.CreateNewDailyRevision(createDaily);
        }

        [HttpPut("UpdateRevisionCycle/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ServiceResponse<GetRevisionCyclesDto>> UpdateRevisionCycle(int id, UpdateRevisionCycleDto dto)
        {
            return await _service.UpdateRevisionCycle(id, dto);
        }

        [HttpDelete("DeleteRevisionCycle/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ServiceResponse<bool>> DeleteRevisionCycle(int id)
        {
            return await _service.DeleteRevisionCycle(id);
        }
    }
}
