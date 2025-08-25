using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.SOS.ToolDtos;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/Analysis_Process/Tools")]
    [ApiController]
    public class ToolsController : ControllerBase
    {
        private readonly ISOS_ProcessRepository _toolService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public ToolsController(ISOS_ProcessRepository tools, IWebHostEnvironment env, IMapper mapper)
        {
            _toolService = tools ??
                  throw new ArgumentNullException(nameof(tools));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTool(ToolForCreateDto toolToCreate)
        {
            var tool = _mapper.Map<Tool>(toolToCreate);
            var createdTool = await _toolService.CreateNewTool(tool);
            var toolDto = _mapper.Map<ToolDto>(createdTool);
            return CreatedAtAction(nameof(GetToolById), new { id = toolDto.ToolId }, toolDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetToolById(int id)
        {
            var tool = await _toolService.GetToolById(id);
            if (tool == null)
                return NotFound();
            var toolDto = _mapper.Map<ToolDto>(tool);
            return Ok(toolDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTools()
        {
            var tools = await _toolService.GetAllTools();
            var toolsDto = _mapper.Map<IEnumerable<ToolDto>>(tools);
            return Ok(toolsDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTool(int id, ToolForUpdateDto toolForUpdate)
        {
            var toolEntity = await _toolService.GetToolById(id);
            if (toolEntity == null)
                return NotFound();

            //_mapper.Map(toolForUpdate, toolEntity);

            int result = await _toolService.UpdateTool(toolForUpdate, toolEntity);
            if (result > 0)
                return NoContent();
            return BadRequest("Failed to update tool.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTool(int id)
        {
            int result = await _toolService.DeleteTool(id);
            if (result > 0)
                return NoContent();
            return NotFound("Tool not found.");
        }

        [HttpGet("Search/{name}")]
        public async Task<IActionResult> SearchTools(string name)
        {
            var tools = await _toolService.GetMatchTools(name);
            return Ok(tools);
        }
    }
}
