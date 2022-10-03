
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.ChecklistQuestionDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/plants/{plantId}/areas")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public AreasController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AreaWithoutNavigationPropertiesDto>>> GetAreas(
                    int plantId)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var areasForPlant = await _supervisorMobilityRepository
                .GetAreasForPlantAsync(plantId);

            return Ok(_mapper.Map<IEnumerable<AreaWithoutNavigationPropertiesDto>>(areasForPlant));
        }

        [HttpGet("{areaId}", Name = "GetArea")]
        public async Task<ActionResult<AreaWithoutNavigationPropertiesDto>> GetArea(
           int plantId, int areaId)
        {
            if (!await _supervisorMobilityRepository.ChecklistCategoryExistAsync(plantId))
            {
                return NotFound();
            }

            var area = await _supervisorMobilityRepository
                .GetAreaForPlantAsync(plantId, areaId);

            if (area == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AreaWithoutNavigationPropertiesDto>(area));
        }

        [HttpPost]
        public async Task<ActionResult<AreaWithoutNavigationPropertiesDto>> CreateArea(
            int plantId,
            AreaForCreationDto area)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var finalArea = _mapper.Map<Area>(area);

            await _supervisorMobilityRepository.AddAreaForPlantAsync(
                plantId, finalArea);

            await _supervisorMobilityRepository.SaveChangesAsync();

            var createdAreaToReturn =
                _mapper.Map<AreaWithoutNavigationPropertiesDto>(finalArea);

            return CreatedAtRoute("GetArea",
                new
                {
                    plantId,
                    areaId = createdAreaToReturn.AreaId
                },
                createdAreaToReturn);
        }

        [HttpPut("{areaid}")]
        public async Task<ActionResult> UpdateArea(int plantId, int areaId,
            AreaForUpdateDto area)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var areaEntity = await _supervisorMobilityRepository
                .GetAreaForPlantAsync(plantId, areaId);
            if (areaEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(area, areaEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{areaid}")]
        public async Task<ActionResult> PartiallyUpdateArea(
            int plantId, int areaId,
            JsonPatchDocument<AreaForUpdateDto> patchDocumentArea)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var areaEntity = await _supervisorMobilityRepository
                .GetAreaForPlantAsync(plantId, areaId);
            if (areaEntity == null)
            {
                return NotFound();
            }

            var areaToPatch = _mapper.Map<AreaForUpdateDto>(areaEntity);

            patchDocumentArea.ApplyTo(areaToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(areaToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(areaToPatch, areaEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{areaId}")]
        public async Task<ActionResult> DeleteArea(int plantId, int areaId)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var areaEntity = await _supervisorMobilityRepository
                .GetAreaForPlantAsync(plantId, areaId);
            if (areaEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteArea(areaEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
