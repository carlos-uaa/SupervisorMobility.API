using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Entities;
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
        readonly IAssyChartService _assyChartService;
        readonly IMapper _mapper;
        public PlantsController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper,
            IAssyChartService assyChartService)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository;
            _mapper = mapper;
            _assyChartService = assyChartService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlantDto>>> GetPlants()
        {
            var plantEntities = await _assyChartService.FetchPlantsAsync();
            return Ok(_mapper.Map<IEnumerable<PlantDto>>(plantEntities));
        }

        [HttpGet("{plantId}", Name = "GetPlant")]
        public async Task<IActionResult> GetPlant(int plantId, bool includeAreas = false)
        {
            //Find Job Observation type
            var plant = await _assyChartService.FetchPlantAsync(plantId, includeAreas);
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
            //Map the object
            var finalPlant = await _assyChartService.CreatePlantAsync(plant);

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
            var plantEntity = await _assyChartService.FetchPlantAsync(plantId);
            if (plantEntity == null)
            {
                return NotFound();
            }

            await _assyChartService.UpdatePlantAsync(plant, plantEntity);

            return Ok();

        }

        [HttpPatch("{plantId}")]
        public async Task<ActionResult> PartiallyUpdatePlant(
            int plantId,
            JsonPatchDocument<PlantForUpdateDto> patchDocumentPlant)
        {
            var plantEntity = await _assyChartService.FetchPlantAsync(plantId);
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

            await _assyChartService.UpdatePlantAsync(plantToPatch, plantEntity);

            return Ok();
        }

        [HttpDelete("{plantId}")]
        public async Task<ActionResult> DeletePlant(int plantId)
        {
            var plantEntity = await _assyChartService.FetchPlantAsync(plantId);
            if (plantEntity == null)
            {
                return NotFound();
            }

            await _assyChartService.RemovePlantAsync(plantEntity);

            return Ok();
        }
    }
}
