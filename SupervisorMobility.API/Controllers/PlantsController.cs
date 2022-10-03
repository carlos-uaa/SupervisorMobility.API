using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Models.GroupDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/plants")]
    [ApiController]
    public class PlantsController : ControllerBase
    {
        readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        readonly IMapper _mapper;
        public PlantsController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlantDto>>> GetPlants()
        {
            var plantEntities = await _supervisorMobilityRepository.GetPlantsAsync();
            return Ok(_mapper.Map<IEnumerable<PlantDto>>(plantEntities));
        }

        [HttpGet("{plantId}", Name = "GetPlant")]
        public async Task<IActionResult> GetPlant(int plantId, bool includeAreas = false)
        {
            //Find Job Observation type
            var plant = await _supervisorMobilityRepository
                .GetPlantAsync(plantId, includeAreas);
            if (plant == null)
            {
                return NotFound();
            }

            if (includeAreas)
            {
                return Ok(_mapper.Map<PlantWithJustAreasDto>(plant));
            }

            return Ok(_mapper.Map<PlantDto>(plant));
        }

        [HttpPost]
        public async Task<ActionResult<PlantDto>> CreatePlant(
            PlantForCreationDto plant)
        {
            //Mpa the pbject
            var finalPlant = _mapper.Map<Entities.Plant>(plant);
            _supervisorMobilityRepository.AddPlant(finalPlant);
            await _supervisorMobilityRepository.SaveChangesAsync();

            var createPlantToReturn =
                _mapper.Map<PlantDto>(finalPlant);

            return CreatedAtRoute("GetPlant",
                new
                {
                    plantId = createPlantToReturn.PlantId
                },
                createPlantToReturn);
        }


        [HttpPut("{plantId}")]
        public async Task<ActionResult> UpdatePlant(int plantId,
            PlantForUpdateDto plant)
        {
            var plantEntity = await _supervisorMobilityRepository.GetPlantAsync(plantId);
            if (plantEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(plant, plantEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();

        }

        [HttpPatch("{plantId}")]
        public async Task<ActionResult> PartiallyUpdatePlant(
            int plantId,
            JsonPatchDocument<PlantForUpdateDto> patchDocumentPlant)
        {
            var plantEntity = await _supervisorMobilityRepository.GetPlantAsync(plantId);
            if (plantEntity == null)
            {
                return NotFound();
            }

            var plantToPatch = _mapper.Map<PlantForUpdateDto>(plantEntity);

            patchDocumentPlant.ApplyTo(plantToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(patchDocumentPlant))
            {
                return BadRequest();
            }

            _mapper.Map(plantToPatch, plantEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{plantId}")]
        public async Task<ActionResult> DeletePlant(int plantId)
        {
            var plantEntity = await _supervisorMobilityRepository.GetPlantAsync(plantId);
            if (plantEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeletePlant(plantEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
