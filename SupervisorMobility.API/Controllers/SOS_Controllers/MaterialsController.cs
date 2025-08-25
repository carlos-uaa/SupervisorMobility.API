using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.SOS.MaterialDtos;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/Analysis_Process/Materials")]
    [ApiController]
    public class MaterialsController : ControllerBase    
    {
        private readonly ISOS_ProcessRepository _MaterialService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public MaterialsController(ISOS_ProcessRepository Materials, IWebHostEnvironment env, IMapper mapper)
        {
            _MaterialService = Materials ??
                  throw new ArgumentNullException(nameof(Materials));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<IActionResult> CreateMaterial(MaterialForCreateDto MaterialToCreate)
        {
            var Material = _mapper.Map<Material>(MaterialToCreate);
            var createdMaterial = await _MaterialService.CreateNewMaterial(Material);
            var MaterialDto = _mapper.Map<MaterialDto>(createdMaterial);
            return CreatedAtAction(nameof(GetMaterialById), new { id = MaterialDto.MaterialId }, MaterialDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterialById(int id)
        {
            var Material = await _MaterialService.GetMaterialById(id);
            if (Material == null)
                return NotFound();
            var MaterialDto = _mapper.Map<MaterialDto>(Material);
            return Ok(MaterialDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMaterials()
        {
            var Materials = await _MaterialService.GetAllMaterials();
            var MaterialsDto = _mapper.Map<IEnumerable<MaterialDto>>(Materials);
            return Ok(MaterialsDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterial(int id, MaterialForUpdateDto MaterialForUpdate)
        {
            var MaterialEntity = await _MaterialService.GetMaterialById(id);
            if (MaterialEntity == null)
                return NotFound();

            //_mapper.Map(MaterialForUpdate, MaterialEntity);

            int result = await _MaterialService.UpdateMaterial(MaterialForUpdate, MaterialEntity);
            if (result > 0)
                return NoContent();
            return BadRequest("Failed to update Material.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            int result = await _MaterialService.DeleteMaterial(id);
            if (result > 0)
                return NoContent();
            return NotFound("Material not found.");
        }

        [HttpGet("Search/{name}")]
        public async Task<IActionResult> SearchMaterials(string name)
        {
            var Materials = await _MaterialService.GetMatchMaterials(name);
            return Ok(Materials);
        }
    }
}
