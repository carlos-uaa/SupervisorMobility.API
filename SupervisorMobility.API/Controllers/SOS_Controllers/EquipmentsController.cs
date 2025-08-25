using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/Analysis_Process/Equipments")]
    [ApiController]
    public class EquipmentsController : ControllerBase
    {
        private readonly ISOS_ProcessRepository _EquipmentService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public EquipmentsController(ISOS_ProcessRepository Equipments, IWebHostEnvironment env, IMapper mapper)
        {
            _EquipmentService = Equipments ??
                  throw new ArgumentNullException(nameof(Equipments));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEquipment(EquipmentForCreateDto EquipmentToCreate)
        {
            var Equipment = _mapper.Map<Equipment>(EquipmentToCreate);
            var createdEquipment = await _EquipmentService.CreateNewEquipment(Equipment);
            var EquipmentDto = _mapper.Map<EquipmentDto>(createdEquipment);
            return CreatedAtAction(nameof(GetEquipmentById), new { id = EquipmentDto.EquipmentId }, EquipmentDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEquipmentById(int id)
        {
            var Equipment = await _EquipmentService.GetEquipmentById(id);
            if (Equipment == null)
                return NotFound();
            var EquipmentDto = _mapper.Map<EquipmentDto>(Equipment);
            return Ok(EquipmentDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEquipments()
        {
            var Equipments = await _EquipmentService.GetAllEquipments();
            var EquipmentsDto = _mapper.Map<IEnumerable<EquipmentDto>>(Equipments);
            return Ok(EquipmentsDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEquipment(int id, EquipmentForUpdateDto EquipmentForUpdate)
        {
            var EquipmentEntity = await _EquipmentService.GetEquipmentById(id);
            if (EquipmentEntity == null)
                return NotFound();

            //_mapper.Map(EquipmentForUpdate, EquipmentEntity);

            int result = await _EquipmentService.UpdateEquipment(EquipmentForUpdate, EquipmentEntity);
            if (result > 0)
                return NoContent();
            return BadRequest("Failed to update Equipment.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            int result = await _EquipmentService.DeleteEquipment(id);
            if (result > 0)
                return NoContent();
            return NotFound("Equipment not found.");
        }

        [HttpGet("Search/{name}")]
        public async Task<IActionResult> SearchEquipments(string name)
        {
            var Equipments = await _EquipmentService.GetMatchEquipments(name);
            return Ok(Equipments);
        }
    }
}
