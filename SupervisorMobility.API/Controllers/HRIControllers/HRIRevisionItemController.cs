using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;

namespace SupervisorMobility.API.Controllers.HRIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HRIRevisionItemController : ControllerBase
    {
        private readonly IHRIRevisionItemService _hriRevisionItemService;

        public HRIRevisionItemController(IHRIRevisionItemService hriRevisionItemService)
        {
            _hriRevisionItemService = hriRevisionItemService;
        }

        [HttpGet("GetAllHRIRevisionItems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<List<GetHRIRevisionItemDto>>>> GetAllHRIRevisionItems()
        {
            var response = await _hriRevisionItemService.GetAllHRIRevisionItems();
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
         [HttpGet("GetHRIRevisionItemById/{id}")]
         [ProducesResponseType(StatusCodes.Status200OK)]
         [ProducesResponseType(StatusCodes.Status404NotFound)]
         [ProducesResponseType(StatusCodes.Status500InternalServerError)]
         public async Task<ActionResult<ServiceResponse<GetHRIRevisionItemDto>>> GetHRIRevisionItemById(int id)
         {
             var response = await _hriRevisionItemService.GetHRIRevisionItemById(id);
             if (response.Data == null)
             {
                 return NotFound(response);
             }
             return Ok(response);
        }

        [HttpPost("CreateHRIRevisionItem")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<GetHRIRevisionItemDto>>> CreateHRIRevisionItem(CreateHRIRevisionItemDto createHRIRevisionItemDto)
        {
            var response = await _hriRevisionItemService.CreateHRIRevisionItem(createHRIRevisionItemDto);
            if (response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("CreateHRIREvisionItemsByHRIId/{hriId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<bool>>> CreateHRIREvisionItemsByHRIId(int hriId, List<CreateHRIRevisionItemDto> createHRIRevisionItemDtos)
        {
            var response = await _hriRevisionItemService.CreateHRIREvisionItemsByHRIId(hriId, createHRIRevisionItemDtos);
            if (response.Data == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("UpdateHRIRevisionItem/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceResponse<GetHRIRevisionItemDto>>> UpdateHRIRevisionItem(int id, UpdateHRIRevisionItemDto updateHRIRevisionItemDto)
        {
            var response = await _hriRevisionItemService.UpdateHRIRevisionItem(id, updateHRIRevisionItemDto);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("DeleteHRIRevisionItem/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteHRIRevisionItem(int id)
        {
            var response = await _hriRevisionItemService.DeleteHRIRevisionItem(id);
            if (response.Data == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllFrequencies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<List<GetFrequencyDto>>>> GetAllFrequencies()
        {
            var response = await _hriRevisionItemService.GetAllFrequencies();
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetFrequencyById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<GetFrequencyDto>>> GetFrequencyById(int id)
        {
            var response = await _hriRevisionItemService.GetFrequencyById(id);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
         }

        [HttpPost("CreateFrequency")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<GetFrequencyDto>>> CreateFrequency(CreateFrequencyDto createFrequencyDto)
        {
            var response = await _hriRevisionItemService.CreateFrequency(createFrequencyDto);
            if (response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
            }

        [HttpPut("UpdateFrequency/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceResponse<GetFrequencyDto>>> UpdateFrequency(int id, UpdateFrequencyDto updateFrequencyDto)
        {
            var response = await _hriRevisionItemService.UpdateFrequency(id, updateFrequencyDto);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("DeleteFrequency/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteFrequency(int id)
        {
            var response = await _hriRevisionItemService.DeleteFrequency(id);
            if (response.Data == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllVeredicts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<List<GetVeredictDto>>>> GetAllVeredicts()
        {
            var response = await _hriRevisionItemService.GetAllVeredicts();
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
         }
         [HttpGet("GetVeredictById/{id}")]
         [ProducesResponseType(StatusCodes.Status200OK)]
         [ProducesResponseType(StatusCodes.Status404NotFound)]
         [ProducesResponseType(StatusCodes.Status500InternalServerError)]
         public async Task<ActionResult<ServiceResponse<GetVeredictDto>>> GetVeredictById(int id)
         {
             var response = await _hriRevisionItemService.GetVeredictById(id);
             if (response.Data == null)
             {
                 return NotFound(response);
             }
             return Ok(response);
          }
         [HttpPost("CreateVeredict")]
         [ProducesResponseType(StatusCodes.Status201Created)]
         [ProducesResponseType(StatusCodes.Status400BadRequest)]
         [ProducesResponseType(StatusCodes.Status500InternalServerError)]
         public async Task<ActionResult<ServiceResponse<GetVeredictDto>>> CreateVeredict(CreateVeredictDto createVeredictDto)
         {
             var response = await _hriRevisionItemService.CreateVeredict(createVeredictDto);
             if (response.Data == null)
             {
                 return BadRequest(response);
             }
             return Ok(response);
        }

            [HttpPut("UpdateVeredict/{id}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<ActionResult<ServiceResponse<GetVeredictDto>>> UpdateVeredict(int id, UpdateVeredictDto updateVeredictDto)
            {
                var response = await _hriRevisionItemService.UpdateVeredict(id, updateVeredictDto);
                if (response.Data == null)
                {
                    return NotFound(response);
                }
                return Ok(response);
        }

        [HttpDelete("DeleteVeredict/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteVeredict(int id)
        {
            var response = await _hriRevisionItemService.DeleteVeredict(id);
            if (response.Data == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllRevisionMethods")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<List<GetRevisionMethodDto>>>> GetAllRevisionMethods()
        {
            var response = await _hriRevisionItemService.GetAllRevisionMethods();
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
         }
         [HttpGet("GetRevisionMethodById/{id}")]
         [ProducesResponseType(StatusCodes.Status200OK)]
         [ProducesResponseType(StatusCodes.Status404NotFound)]
         [ProducesResponseType(StatusCodes.Status500InternalServerError)]
         public async Task<ActionResult<ServiceResponse<GetRevisionMethodDto>>> GetRevisionMethodById(int id)
         {
             var response = await _hriRevisionItemService.GetRevisionMethodById(id);
             if (response.Data == null)
             {
                 return NotFound(response);
             }
             return Ok(response);
          }
         [HttpPost("CreateRevisionMethod")]
         [ProducesResponseType(StatusCodes.Status201Created)]
         [ProducesResponseType(StatusCodes.Status400BadRequest)]
         [ProducesResponseType(StatusCodes.Status500InternalServerError)]
         public async Task<ActionResult<ServiceResponse<GetRevisionMethodDto>>> CreateRevisionMethod(CreateRevisionMethodDto createRevisionMethodDto)
         {
             var response = await _hriRevisionItemService.CreateRevisionMethod(createRevisionMethodDto);
             if (response.Data == null)
             {
                 return BadRequest(response);
             }
             return Ok(response);
        }

        [HttpPut("UpdateRevisionMethod/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceResponse<GetRevisionMethodDto>>> UpdateRevisionMethod(int id, UpdateRevisionMethodDto updateRevisionMethodDto)
        {
            var response = await _hriRevisionItemService.UpdateRevisionMethod(id, updateRevisionMethodDto);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("DeleteRevisionMethod/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteRevisionMethod(int id)
        {
            var response = await _hriRevisionItemService.DeleteRevisionMethod(id);
            if (response.Data == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}