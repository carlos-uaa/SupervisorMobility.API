using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/plants/{plantId}/areas/{areaId}/operations")]
    [ApiController]
    public class OperationsController :  ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public OperationsController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperationWithoutNavigationPropertiesDto>>> GetOperations(
                    int plantId, int areaId)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound();
            }

            var areasForPlant = await _supervisorMobilityRepository
                .GetOperationsForAreaAsync(areaId);

            return Ok(_mapper.Map<IEnumerable<OperationWithoutNavigationPropertiesDto>>(areasForPlant));
        }

        [HttpGet("{operationId}", Name = "GetOperation")]
        public async Task<ActionResult<OperationWithoutNavigationPropertiesDto>> GetOperation(
           int plantId, int areaId, int operationId)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound();
            }

            var operation = await _supervisorMobilityRepository
                .GetOperationForAreaAsync(areaId, operationId);

            if (operation == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<OperationWithoutNavigationPropertiesDto>(operation));
        }

        [HttpPost]
        public async Task<ActionResult<OperationWithoutNavigationPropertiesDto>> CreateOperation(
            int plantId,
            int areaId,
            OperationForCreationDto operation)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound();
            }

            var finalOperation = _mapper.Map<Operation>(operation);

            await _supervisorMobilityRepository.AddOperationForPlantAsync(plantId,
                areaId, finalOperation);

            await _supervisorMobilityRepository.SaveChangesAsync();

            var createdOperationToReturn =
                _mapper.Map<OperationWithoutNavigationPropertiesDto>(finalOperation);

            return CreatedAtRoute("GetOperation",
                new
                {
                    plantId,
                    areaId,
                    operationId = createdOperationToReturn.OperationId
                },
                createdOperationToReturn);
        }

        [HttpPut("{operationid}")]
        public async Task<ActionResult> UpdateOperation(int plantId, int areaId,
            int operationId,
            OperationForUpdateDto operation)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound();
            }

            var operationEntity = await _supervisorMobilityRepository
                .GetOperationForAreaAsync(areaId, operationId);
            if (operationEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(operation, operationEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{operationid}")]
        public async Task<ActionResult> PartiallyUpdateOperation(
            int plantId, int areaId, int operationId,
            JsonPatchDocument<OperationForUpdateDto> patchDocumentOperation)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound();
            }

            var operationEntity = await _supervisorMobilityRepository
                .GetOperationForAreaAsync(areaId, operationId);
            if (operationEntity == null)
            {
                return NotFound();
            }

            var operationToPatch = _mapper.Map<OperationForUpdateDto>(operationEntity);

            patchDocumentOperation.ApplyTo(operationToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(operationToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(operationToPatch, operationEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{operationId}")]
        public async Task<ActionResult> DeleteOperation(int plantId, int areaId, int operationId)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(areaId))
            {
                return NotFound();
            }

            var operationEntity = await _supervisorMobilityRepository
                .GetOperationForAreaAsync(areaId, operationId);
            if (operationEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteOperation(operationEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
